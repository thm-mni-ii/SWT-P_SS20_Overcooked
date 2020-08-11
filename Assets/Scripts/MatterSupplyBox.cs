using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    /// <summary>
    /// Contains all functions concerning the matter supplybox.
    /// </summary>
    public class MatterSupplyBox : NetworkBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField] MeshRenderer matterDisplayRenderer;

        [Header("Settings")]
        [SerializeField] Matter containedMatter;
        [SerializeField] bool canTakeBackItems = true;
        [SerializeField] string matterDisplayTextureField = "_Back";


        /// <summary>
        /// The material used to display the matter icon.
        /// This reference is needed to be able to change the displayed texture.
        /// </summary>
        private Material matterDisplayMaterial;


        /// <summary>
        /// matterDisplayMaterial is initialized and <see cref="SetContainedMatter(Matter)"/> is called.
        /// </summary>
        private void Awake()
        {
            if (this.matterDisplayRenderer != null)
                this.matterDisplayMaterial = this.matterDisplayRenderer.material;
            this.SetContainedMatter(this.containedMatter);
        }

        /// <summary>
        /// Checks if interactor is server
        /// Checks if interactor is already holding an object -
        /// if it is not holding an object the respective matter object is created, <see cref="Interactor.HeldObject"/> is called
        /// and then <see cref="RpcGiveMatterObjectToInteractor(NetworkIdentity, NetworkIdentity)"/> is called
        /// with the given interactor and the created matter as parameters.
        /// If it is holding an object and the container can take this object back the matter object will be destroyed
        /// and the interactor will not hold an object anymore.
        /// </summary>
        /// <param name="interactor"></param>
        public void Interact(Interactor interactor)
        {
            if (this.isServer)
            {
                if (!interactor.IsHoldingObject)
                {
                    if (this.containedMatter != null)
                    {
                        GameObject instantiatedMatter = GameObject.Instantiate(this.containedMatter.GetPrefab(), this.transform.position, Quaternion.identity);
                        NetworkServer.Spawn(instantiatedMatter);
                        interactor.SetHeldObject(instantiatedMatter.GetComponent<PickableObject>());

                        this.RpcGiveMatterObjectToInteractor(instantiatedMatter.GetComponent<NetworkIdentity>(), interactor.GetComponent<NetworkIdentity>());
                    }
                }
                else if (this.canTakeBackItems)
                {
                    MatterObject matterObject = interactor.HeldObject.GetComponent<MatterObject>();

                    if (matterObject != null && (this.containedMatter == null || this.containedMatter.Equals(matterObject.Matter)))
                    {
                        interactor.SetHeldObject(null);
                        this.RpcGiveMatterObjectToInteractor(null, interactor.GetComponent<NetworkIdentity>());
                        NetworkServer.Destroy(matterObject.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the given matter as the contained matter of supplybox.
        /// </summary>
        /// <param name="matter">matter to be contained in supplybox</param>
        public void SetContainedMatter(Matter matter)
        {
            this.containedMatter = matter;
            if (this.matterDisplayMaterial != null)
                this.matterDisplayMaterial.SetTexture(this.matterDisplayTextureField, matter != null ? matter.GetIcon().texture : null);
        }


        [ClientRpc]
        /// <summary>
        /// Is only called by server.
        /// Checks if given iterator and matter is not null and calls <see cref="Interactor.SetHeldObject(PickableObject)"/> 
        /// with given matterObject as parameter
        /// </summary>
        /// <param name="matterObject">matter object to be interacted with</param>
        /// <param name="interactor">object which is able to interact</param>
        private void RpcGiveMatterObjectToInteractor(NetworkIdentity matterObject, NetworkIdentity interactor)
        {
            if (interactor != null)
            {
                Interactor i = interactor.GetComponent<Interactor>();

                if (i != null)
                    i.SetHeldObject(matterObject != null ? matterObject.GetComponent<PickableObject>() : null);
            }
        }
    }
}
