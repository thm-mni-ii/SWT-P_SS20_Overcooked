using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Underconnected
{
    public class MatterSupplyBox : NetworkBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField] MeshRenderer matterDisplayRenderer;

        [Header("Settings")]
        [SerializeField] Matter containedMatter;
        [SerializeField] bool canTakeBackItems = true;
        [SerializeField] string matterDisplayTextureField = "_Back";


        private Material matterDisplayMaterial;


        private void Awake()
        {
            if (this.matterDisplayRenderer != null)
                this.matterDisplayMaterial = this.matterDisplayRenderer.material;
            this.SetContainedMatter(this.containedMatter);
        }


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


        public void SetContainedMatter(Matter matter)
        {
            this.containedMatter = matter;
            if (this.matterDisplayMaterial != null)
                this.matterDisplayMaterial.SetTexture(this.matterDisplayTextureField, matter != null ? matter.GetIcon().texture : null);
        }


        [ClientRpc]
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
