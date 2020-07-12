using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ElementSupplyBox : NetworkBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] MeshRenderer elementDisplayRenderer;

    [Header("Settings")]
    [SerializeField] Matter containedMatter;
    [SerializeField] string elementDisplayTextureField = "_Back";


    private Material elementDisplayMaterial;


    private void Awake()
    {
        this.elementDisplayMaterial = this.elementDisplayRenderer.material;
        this.SetContainedMatter(this.containedMatter);
    }


    public void Interact(Interactor interactor)
    {
        if (this.isServer)
        {
            if (!interactor.IsHoldingObject)
            {
                GameObject instantiatedElement = GameObject.Instantiate(this.containedMatter.GetPrefab(), this.transform.position, Quaternion.identity);
                NetworkServer.Spawn(instantiatedElement);
                interactor.SetHeldObject(instantiatedElement.GetComponent<PickableObject>());

                this.RpcGiveElementToInteractor(instantiatedElement.GetComponent<NetworkIdentity>(), interactor.GetComponent<NetworkIdentity>());
            }
        }
    }


    public void SetContainedMatter(Matter matter)
    {
        this.containedMatter = matter;
        this.elementDisplayMaterial.SetTexture(this.elementDisplayTextureField, matter != null ? matter.GetIcon().texture : null);
    }


    [ClientRpc]
    private void RpcGiveElementToInteractor(NetworkIdentity element, NetworkIdentity interactor)
    {
        Interactor i = interactor.GetComponent<Interactor>();

        if (i != null)
            i.SetHeldObject(element.GetComponent<PickableObject>());
    }
}
