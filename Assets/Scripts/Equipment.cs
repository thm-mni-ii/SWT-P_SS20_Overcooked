using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// Represents a piece of lab equipment that can be interacted with.
    /// </summary>
    public class Equipment : ModifiableObject
    {
        [Header("References")]
        [SerializeField] GameObject inputContainer;
        [SerializeField] GameObject outputContainer;
        [SerializeField] Light fireLight;
        [SerializeField] ParticleSystem fireParticles;

        [Header("Settings")]
        [SerializeField] bool allowForeignMattersInRecipes;
        [SerializeField] Recipe[] allRecipes;


        /// <summary>
        /// Stores all the inserted matter objects.
        /// </summary>
        private List<MatterObject> insertedObjects;
        /// <summary>
        /// Stores all the inserted matter objects from <see cref="insertedObjects"/> as simple matters.
        /// Used inside of <see cref="InsertMatterObject(MatterObject)"/> to determine which recipe can be made out of the contained matters.
        /// </summary>
        private List<Matter> insertedMatter;
        /// <summary>
        /// The recipe that is currently being processed by this equipment.
        /// </summary>
        private Recipe recipeInProgress;
        /// <summary>
        /// The output slot of this equipment.
        /// </summary>
        private MatterObject outputObject;


        private void Awake()
        {
            this.insertedObjects = new List<MatterObject>();
            this.insertedMatter = new List<Matter>();
            this.recipeInProgress = null;
            this.outputObject = null;
        }


        /// <summary>
        /// Interacts with this object.
        /// Attempts to insert a matter object held by <paramref name="interactor"/> or take a matter object from the output slot.
        /// If the interactor does not hold anything and the output slot is empty, this method will try take the last inserted object and give it to the interactor.
        /// </summary>
        /// <param name="interactor">The initiator of this interaction.</param>
        public override void Interact(Interactor interactor)
        {
            if (this.IsActivated)
                return;

            PickableObject heldObject = interactor.HeldObject;

            if (heldObject != null)
            {
                if (this.outputObject == null)
                {
                    MatterObject matterObject = heldObject.GetComponent<MatterObject>();

                    if (matterObject != null)
                    {
                        interactor.SetHeldObject(null);
                        this.InsertMatterObject(matterObject);
                    }
                }
            }
            else if (interactor.HeldObject == null)
            {
                MatterObject toTake = this.outputObject != null ? this.TakeOutputObject() : this.insertedObjects.Count > 0 ? this.TakeLastInsertedObject() : null;

                if (toTake != null)
                    interactor.SetHeldObject(toTake.GetComponent<PickableObject>());
            }
        }


        /// <summary>
        /// Called when the timer has finished.
        /// Disables all effects, hides the progress bar and calls <see cref="CompleteRecipe(Recipe)"/>.
        /// </summary>
        protected override void OnTimerFinish()
        {
            if (!this.IsFinished)
            {
                this.fireLight.enabled = false;
                this.fireParticles.Stop();

                this.IsActivated = false;
                this.IsFinished = true;
                this.ObjectInfoCanvas.gameObject.SetActive(false);

                if (this.isServer)
                    this.CompleteRecipe(this.recipeInProgress);
            }
        }


        /// <summary>
        /// Inserts the given matter object into this equipment.
        /// Checks whether a recipe matches the inserted elements and starts the production process, shows the progress bar and enables effects if a matching recipe has been found.
        /// </summary>
        /// <param name="matterObject"></param>
        private void InsertMatterObject(MatterObject matterObject)
        {
            if (matterObject != null)
            {
                matterObject.DisablePhysics();
                matterObject.transform.SetParent(this.inputContainer.transform, false);
                matterObject.transform.localPosition = Vector3.zero;
                matterObject.transform.localRotation = Quaternion.identity;
                this.insertedObjects.Add(matterObject);
                this.insertedMatter.Add(matterObject.Matter);

                foreach (Recipe recipe in this.allRecipes)
                {
                    if (recipe.MatchInputs(this.insertedMatter, this.allowForeignMattersInRecipes) == RecipeMatchState.FullMatch)
                    {
                        this.recipeInProgress = recipe;
                        break;
                    }
                }
                if (this.recipeInProgress != null)
                {
                    this.fireLight.enabled = true;
                    this.fireParticles.Play();

                    this.IsActivated = false;
                    this.IsFinished = false;
                    this.OnTimerStart(null);
                }
            }
        }
        /// <summary>
        /// Ejects the matter object inside the output slot and returns it.
        /// </summary>
        /// <returns>The output matter object or `null` if none.</returns>
        private MatterObject TakeOutputObject()
        {
            if (this.outputObject != null)
            {
                MatterObject toTake = this.outputObject;
                toTake.EnablePhysics();
                toTake.transform.SetParent(this.transform, false);

                this.outputObject = null;
                return toTake;
            }

            return null;
        }
        /// <summary>
        /// Ejects the last inserted matter object, removes it from the input and returns it.
        /// </summary>
        /// <returns>The last inserted matter object or `null` if none.</returns>
        private MatterObject TakeLastInsertedObject()
        {
            if (this.insertedObjects.Count > 0)
            {
                int index = this.insertedObjects.Count - 1;
                MatterObject toTake = this.insertedObjects[index];
                toTake.EnablePhysics();
                toTake.transform.SetParent(this.transform, false);

                this.insertedMatter.RemoveAt(index);
                this.insertedObjects.RemoveAt(index);

                return toTake;
            }

            return null;
        }

        /// <summary>
        /// Finishes the given recipe and sends the results to the clients.
        /// Removes matters from the input that have been consumed by the recipe.
        /// Adds the result matter to the output slot.
        /// Can only be called on a server.
        /// </summary>
        /// <param name="recipe">The finished recipe.</param>
        [Server]
        private void CompleteRecipe(Recipe recipe)
        {
            this.RemoveFromInput(recipe.Inputs);
            this.AddToOutput(recipe.Output);

            this.recipeInProgress = null;
            this.RpcCompleteRecipe();
        }
        /// <summary>
        /// Adds the given matter to the output slot and synchronizes it with the clients.
        /// Can only be called on the server.
        /// </summary>
        /// <param name="matter">The matter to add.</param>
        [Server]
        private void AddToOutput(Matter matter)
        {
            if (matter != null)
            {
                GameObject resultMatter = GameObject.Instantiate(matter.GetPrefab());

                this.outputObject = resultMatter.GetComponent<MatterObject>();
                this.outputObject.DisablePhysics();

                resultMatter.transform.SetParent(this.outputContainer.transform, false);
                resultMatter.transform.localPosition = Vector3.zero;
                resultMatter.transform.localRotation = Quaternion.identity;

                NetworkServer.Spawn(resultMatter);
                this.RpcAddOutput(resultMatter.GetComponent<NetworkIdentity>());
            }
        }
        /// <summary>
        /// Removes the given matters from the input and synchronizes it with the clients.
        /// Can only be called on the server.
        /// </summary>
        /// <param name="matters">The matters to remove from the input.</param>
        [Server]
        private void RemoveFromInput(Matter[] matters)
        {
            List<int> removeIndices = new List<int>(matters.Length);
            foreach (Matter toRemove in matters)
            {
                int insertedSlotIndex = this.insertedMatter.IndexOf(toRemove);
                while (insertedSlotIndex >= 0 && removeIndices.Contains(insertedSlotIndex) && insertedSlotIndex + 1 < this.insertedMatter.Count)
                    insertedSlotIndex = this.insertedMatter.IndexOf(toRemove, insertedSlotIndex + 1);

                if (insertedSlotIndex >= 0 && insertedSlotIndex < this.insertedMatter.Count)
                {
                    NetworkServer.Destroy(this.insertedObjects[insertedSlotIndex].gameObject);

                    this.insertedMatter.RemoveAt(insertedSlotIndex);
                    this.insertedObjects.RemoveAt(insertedSlotIndex);
                    removeIndices.Add(insertedSlotIndex);
                }
            }
            this.RpcRemoveFromInput(removeIndices.ToArray());
        }


        /// <summary>
        /// Tells all clients to reset the currently processed recipe.
        /// Sent by the server to all clients.
        /// </summary>
        [ClientRpc]
        private void RpcCompleteRecipe()
        {
            this.recipeInProgress = null;
        }
        /// <summary>
        /// Tells all clients to remove elements from the input at the given indices.
        /// Sent by the server to all clients.
        /// </summary>
        /// <param name="indices">The element indices to remove.</param>
        [ClientRpc]
        private void RpcRemoveFromInput(int[] indices)
        {
            if (this.isClientOnly)
            {
                try
                {
                    foreach (int index in indices)
                    {
                        this.insertedMatter.RemoveAt(index);
                        this.insertedObjects.RemoveAt(index);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Cannot remove given indices from input list. Clearing entire list instead.", this);
                    Debug.LogException(ex, this);

                    this.insertedObjects.Clear();
                    this.insertedMatter.Clear();
                }
            }
        }
        /// <summary>
        /// Tells all clients to clear the entire input.
        /// Sent by the server to all clients.
        /// Currently not used anywhere.
        /// </summary>
        [ClientRpc]
        private void RpcClearInput()
        {
            if (this.isClientOnly)
            {
                this.insertedObjects.Clear();
                this.insertedMatter.Clear();
            }
        }
        /// <summary>
        /// Tells all clients to assign the given object to the output slot.
        /// Sent by the server to all clients.
        /// </summary>
        /// <param name="outputObject">The <see cref="NetworkIdentity"/> component of the new output object. Expects to have a <see cref="MatterObject"/> component.</param>
        [ClientRpc]
        private void RpcAddOutput(NetworkIdentity outputObject)
        {
            if (this.isClientOnly && outputObject != null)
            {
                MatterObject matterObject = outputObject.GetComponent<MatterObject>();
                matterObject.DisablePhysics();

                outputObject.transform.SetParent(this.outputContainer.transform, false);
                outputObject.transform.localPosition = Vector3.zero;
                outputObject.transform.localRotation = Quaternion.identity;

                this.outputObject = matterObject;
            }
        }
    }
}
