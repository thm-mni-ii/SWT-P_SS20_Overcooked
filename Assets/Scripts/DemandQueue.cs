using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Manages all functionalities concerning the queue of demanded matters like adding, removing and delivering.
    /// </summary>
    public class DemandQueue : NetworkBehaviour
    {
        /// <summary>
        /// Returns all the demands that are currently inside this demand queue.
        /// </summary>
        public SortedDictionary<int, Demand>.ValueCollection CurrentDemands => this.currentDemands.Values;


        /// <summary>
        /// Fired when a demand is added to this queue.
        /// Parameters: The demand that has been added.
        /// </summary>
        public event UnityAction<Demand> OnDemandAdded;
        /// <summary>
        /// Fired when a demand is removed from this queue.
        /// Parameters: The demand that has been removed.
        /// </summary>
        public event UnityAction<Demand> OnDemandRemoved;
        /// <summary>
        /// Fired when a demand's time limit is reached and it will be removed.
        /// <see cref="OnDemandRemoved"/> will be fired after this event.
        /// Parameters: The expired demand.
        /// </summary>
        public event UnityAction<Demand> OnDemandExpired;


        /// <summary>
        /// Holds the ID for the next demand that will spawn.
        /// Is only used by the server to make sure that each demand that is spawned has a unique ID.
        /// </summary>
        private int nextDemandID;
        /// <summary>
        /// A dictionary containing all the current demands.
        /// The key is the demand's ID, the value is the <see cref="Demand"/> object itself.
        /// </summary>
        private SortedDictionary<int, Demand> currentDemands;
        /// <summary>
        /// Holds the demands that are flagged for removal.
        /// This is needed because demands can only detect whether they have expired when <see cref="Update"/> iterates through them and calls their <see cref="Demand.Update(float)"/> method.
        /// Expired demands should be removed but we can't do that while iterating through them.
        /// This is why we store them here first and then remove them in the next frame.
        /// </summary>
        private List<Demand> demandsToRemove;


        private void Awake()
        {
            this.nextDemandID = 0;
            this.currentDemands = new SortedDictionary<int, Demand>();
            this.demandsToRemove = new List<Demand>();
        }
        private void Update()
        {
            // Remove the demands that have been flagged for removal first
            if (this.demandsToRemove.Count > 0)
            {
                foreach (Demand toRemove in this.demandsToRemove)
                    this.currentDemands.Remove(toRemove.ID);
                this.demandsToRemove.Clear();
            }

            // Then iterate over the still existing demands and update them, potentially flagging more demands for removal (when their OnExpired event is fired)
            var enumerator = this.currentDemands.GetEnumerator();
            while (enumerator.MoveNext())
                enumerator.Current.Value.Update(Time.deltaTime);
        }


        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            bool dataWritten = base.OnSerialize(writer, initialState);

            if (initialState)
            {
                writer.WriteInt32(this.currentDemands.Count);

                var enumerator = this.currentDemands.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    writer.WriteInt32(enumerator.Current.Value.ID);
                    writer.WriteString(enumerator.Current.Value.Matter.GetID());
                    writer.WriteSingle(enumerator.Current.Value.TimeLimit);

                    if (enumerator.Current.Value.HasTimeLimit)
                        writer.WriteSingle(enumerator.Current.Value.TimeLeft);
                }

                dataWritten = true;
            }

            return dataWritten;
        }
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            base.OnDeserialize(reader, initialState);

            if (initialState)
            {
                int amountOfDemands = reader.ReadInt32();
                Demand demand;

                for (int i = 0; i < amountOfDemands; i++)
                {
                    demand = new Demand(reader.ReadInt32(), Matter.GetByID(reader.ReadString()), reader.ReadSingle());

                    if (demand.HasTimeLimit)
                        demand.SetTimeLeft(reader.ReadSingle());

                    this.AcceptDemand(demand);
                }
            }
        }


        /// <summary>
        /// Adds a matter to the demand queue with the given time limit and synchronizes it with the clients.
        /// Can only be called on the server (since demand spawn is exclusively server side).
        /// </summary>
        /// <param name="matter">The matter to add to the demand queue.</param>
        /// <param name="timeLimit">The time limit for the demand. `0` if infinite.</param>
        [Server]
        public void AddDemand(Matter matter, float timeLimit = 0)
        {
            Demand demand = new Demand(this.nextDemandID++, matter, timeLimit);
            demand.OnExpired += this.Demand_OnExpired_Server;

            this.AcceptDemand(demand);
        }
        /// <summary>
        /// Tries to deliver a matter and remove its demand from this demand queue.
        /// Synchronizes the removal process with the clients.
        /// Can only be called on the server.
        /// </summary>
        /// <param name="matter">The matter to deliver.</param>
        /// <returns>The demand that has been delivered or `null` if none matched.</returns>
        [Server]
        public Demand Deliver(Matter matter)
        {
            Demand demand = this.GetDemand(matter);

            if (demand != null)
            {
                demand.SetDelivered();
                this.RemoveDemand(demand);
                return demand;
            }

            return null;
        }
        /// <summary>
        /// Returns the oldest demand for the given matter.
        /// </summary>
        /// <param name="matter">The matter to find a demand for.</param>
        /// <returns>The oldest demand for <paramref name="matter"/> or `null` if none found.</returns>
        public Demand GetDemand(Matter matter)
        {
            if (matter != null)
            {
                var enumerator = this.currentDemands.GetEnumerator();

                while (enumerator.MoveNext())
                    if (enumerator.Current.Value.Matter.Equals(matter) && !this.demandsToRemove.Contains(enumerator.Current.Value))
                        return enumerator.Current.Value;
            }

            return null;
        }


        /// <summary>
        /// Adds the given demand to the queue and synchronizes it with the clients if called on the server.
        /// </summary>
        /// <param name="demand">The demand to add to the queue.</param>
        private void AcceptDemand(Demand demand)
        {
            this.currentDemands.Add(demand.ID, demand);
            this.OnDemandAdded?.Invoke(demand);

            if (this.isServer)
                this.RpcAcceptDemand(demand.ID, demand.Matter.GetID(), demand.TimeLimit);
        }
        /// <summary>
        /// Removes the given demand from the queue and synchronizes it with the clients if called on the server.
        /// Adds the given demand to the removal queue (<see cref="demandsToRemove"/>).
        /// </summary>
        /// <param name="demand">The demand to remove.</param>
        private void RemoveDemand(Demand demand)
        {
            this.demandsToRemove.Add(demand);
            this.OnDemandRemoved?.Invoke(demand);

            if (this.isServer)
                this.RpcRemoveDemand(demand.ID);
        }


        /// <summary>
        /// Tells the clients to add a new demand with the given ID, matter ID and time limit.
        /// Sent by the server to all clients.
        /// </summary>
        /// <param name="demandID">The ID to assign to the new demand.</param>
        /// <param name="matterID">The ID of the matter for the new demand.</param>
        /// <param name="timeLimit">The time limit for the demand.</param>
        [ClientRpc]
        private void RpcAcceptDemand(int demandID, string matterID, float timeLimit)
        {
            if (this.isClientOnly)
            {
                Matter targetMatter = Matter.GetByID(matterID);

                if (targetMatter != null)
                    this.AcceptDemand(new Demand(demandID, targetMatter, timeLimit));
                else
                    Debug.LogError($"Cannot accept nonexisting matter with id '{matterID}'.");
            }
        }
        /// <summary>
        /// Tells the clients to remove the demand with the given ID from the demand queue.
        /// Sent by the server to all clients.
        /// </summary>
        /// <param name="demandID">The ID of the demand to remove from the queue.</param>
        [ClientRpc]
        private void RpcRemoveDemand(int demandID)
        {
            if (this.isClientOnly)
            {
                Demand toRemove;
                if (this.currentDemands.TryGetValue(demandID, out toRemove))
                    this.RemoveDemand(toRemove);
            }
        }


        /// <summary>
        /// Called when a demand expires.
        /// Fires <see cref="OnDemandExpired"/> and removes the demand from the queue.
        /// Only called on the server since this event is only registered on the server side.
        /// </summary>
        /// <param name="demandedMatterUI">The demand that has expired.</param>
        private void Demand_OnExpired_Server(Demand demand)
        {
            this.OnDemandExpired?.Invoke(demand);
            this.RemoveDemand(demand);
        }
    }
}
