using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mirror;

public class TeamBase : MonoBehaviour
{
    [SerializeField] Team team;
    [SerializeField] string albedoPropertyName = "_Albedo";
    [SerializeField] MeshRenderer[] allMeshRenderers;
    [SerializeField] Transform flagSpawn;
    [SerializeField] GameObject flagPrefab;


    public Team Team { get => this.team; }
    public Flag TeamFlag { get; private set; }
    public Transform FlagSpawn { get => this.flagSpawn; }

    private Material[] allMaterials;


    private void Awake()
    {
        if (this.team != null)
        {
            this.team.Register(this);

            this.UpdateMaterialReferences();
            this.UpdateMeshColor();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (PrefabUtility.GetPrefabInstanceStatus(this.gameObject) != PrefabInstanceStatus.NotAPrefab)
        {
            this.UpdateMaterialReferences();
            this.UpdateMeshColor();
        }
    }
#endif


    public void SpawnFlag()
    {
        if (this.TeamFlag == null)
        {
            this.TeamFlag = GameObject.Instantiate(this.flagPrefab).GetComponent<Flag>();
            this.TeamFlag.SetBase(this);

            NetworkServer.Spawn(this.TeamFlag.gameObject);
            this.TeamFlag.ResetFlag();
        }
    }


    private void UpdateMaterialReferences()
    {
        if (this.allMeshRenderers.Length > 0)
        {
            this.allMaterials = new Material[this.allMeshRenderers.Length];
            for (int i = 0; i < this.allMeshRenderers.Length; i++)
                this.allMaterials[i] = this.allMeshRenderers[i].sharedMaterial;
        }
        else
            this.allMaterials = null;
    }
    private void UpdateMeshColor()
    {
        if (this.allMaterials != null)
            for (int i = 0; i < this.allMaterials.Length; i++)
                this.allMaterials[i].SetColor(this.albedoPropertyName, this.team.TeamColor);
    }
}
