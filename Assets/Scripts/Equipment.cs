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
    [SerializeField] bool allowForeignMattersInRecipes;
    [SerializeField] Recipe[] allRecipes;


    private List<MatterObject> insertedObjects;
    private List<Matter> insertedMatter;
    private Recipe recipeInProgress;
    private MatterObject outputObject;


    private void Awake()
    {
        this.insertedObjects = new List<MatterObject>();
        this.insertedMatter = new List<Matter>();
        this.recipeInProgress = null;
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

    private void CompleteRecipe(Recipe recipe)
    {
        if (this.isServer)
        {
            this.RemoveFromInput(recipe.Inputs);
            this.AddToOutput(recipe.Output);

            this.recipeInProgress = null;
            this.RpcCompleteRecipe();
        }
    }
    private void AddToOutput(Matter matter)
    {
        if (this.isServer && matter != null)
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
    private void RemoveFromInput(Matter[] matters)
    {
        if (this.isServer)
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
    }


    [ClientRpc]
    private void RpcCompleteRecipe()
    {
        this.recipeInProgress = null;
    }
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
                Debug.LogError($"Cannot remove given indices from input list. Clearing entire list instead.", this);
                Debug.LogException(ex, this);

                this.insertedObjects.Clear();
                this.insertedMatter.Clear();
            }
        }
    }
    [ClientRpc]
    private void RpcClearInput()
    {
        if (this.isClientOnly)
        {
            this.insertedObjects.Clear();
            this.insertedMatter.Clear();
        }
    }
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
