using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : ModifiableObject
{
    [Header("References")]
    [SerializeField] GameObject inputContainer;
    [SerializeField] GameObject outputContainer;
    [SerializeField] Light fireLight;
    [SerializeField] ParticleSystem fireParticles;

    [Header("Settings")]
    [SerializeField] Recipe[] acceptedElements;
    [SerializeField] Recipe resultElement;


    private List<ElementObject> insertedObjects;
    private ElementObject outputObject;


    private void Awake()
    {
        this.insertedObjects = new List<ElementObject>();
        this.outputObject = null;
    }


    public override void Interact(Interactor interactor)
    {
        if (this.IsActivated)
            return;

        PickableObject heldObject = interactor.HeldObject;

        if (heldObject != null)
        {
            if (this.outputObject == null)
            {
                ElementObject elementObject = heldObject.GetComponent<ElementObject>();

                if (elementObject != null && this.AcceptsElement(elementObject.Element))
                {
                    interactor.SetHeldObject(null);

                    elementObject.DisablePhysics();
                    heldObject.transform.SetParent(this.inputContainer.transform, false);
                    heldObject.transform.localPosition = Vector3.zero;
                    heldObject.transform.localRotation = Quaternion.identity;
                    this.insertedObjects.Add(elementObject);

                    // TODO: Check actual recipes
                    if (this.insertedObjects.Count >= 3)
                    {
                        this.fireLight.enabled = true;
                        this.fireParticles.Play();

                        this.IsActivated = false;
                        this.IsFinished = false;
                        this.OnTimerStart(interactor);
                    }
                }
            }
        }
        else if (this.outputObject != null)
        {
            if (interactor.HeldObject == null)
            {
                this.outputObject.EnablePhysics();
                this.outputObject.transform.SetParent(this.transform, false);
                interactor.SetHeldObject(this.outputObject.GetComponent<PickableObject>());

                this.outputObject = null;
            }
        }
    }

    public bool AcceptsElement(Recipe element)
    {
        if (element != null)
        {
            foreach (Recipe acceptedElement in this.acceptedElements)
                if (acceptedElement.Equals(element))
                    return true;
        }

        return false;
    }


    protected override void OnTimerFinish()
    {
        if (!this.IsFinished)
        {
            this.fireLight.enabled = false;
            this.fireParticles.Stop();

            this.IsActivated = false;
            this.IsFinished = true;
            this.ObjectInfoCanvas.gameObject.SetActive(false);

            this.ClearInput();
            this.AddToOutput(this.resultElement);
        }
    }


    private void AddToOutput(Recipe element)
    {
        if (this.isServer && element != null)
        {
            GameObject resultElement = GameObject.Instantiate(element.GetPrefab());

            this.outputObject = resultElement.GetComponent<ElementObject>();
            this.outputObject.DisablePhysics();

            resultElement.transform.SetParent(this.outputContainer.transform, false);
            resultElement.transform.localPosition = Vector3.zero;
            resultElement.transform.localRotation = Quaternion.identity;

            NetworkServer.Spawn(resultElement);
            this.RpcAddOutput(this.outputObject.GetComponent<NetworkIdentity>());
        }
    }
    private void ClearInput()
    {
        if (this.isServer)
        {
            foreach (ElementObject element in this.insertedObjects)
                NetworkServer.Destroy(element.gameObject);
            this.insertedObjects.Clear();

            this.RpcClearInput();
        }
    }


    [ClientRpc]
    private void RpcClearInput()
    {
        this.insertedObjects.Clear();
    }
    [ClientRpc]
    private void RpcAddOutput(NetworkIdentity outputObject)
    {
        if (outputObject != null)
        {
            ElementObject elementObject = outputObject.GetComponent<ElementObject>();
            elementObject.DisablePhysics();

            outputObject.transform.SetParent(this.outputContainer.transform, false);
            outputObject.transform.localPosition = Vector3.zero;
            outputObject.transform.localRotation = Quaternion.identity;

            this.outputObject = elementObject;
        }
    }
}
