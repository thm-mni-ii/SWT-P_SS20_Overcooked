using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExposeShader : MonoBehaviour
{
    [SerializeField] Camera currentCamera;
    [SerializeField] string playerViewportPositionProperty = "PlayerViewportPosition";


    public static PlayerExposeShader Instance { get; private set; }


    public PlayerControls Player { get; set; } = null;


    private void Awake()
    {
        if (PlayerExposeShader.Instance == null)
            PlayerExposeShader.Instance = this;
        else
            GameObject.Destroy(this.gameObject);
    }


    private void LateUpdate()
    {
        if (this.Player != null)
            Shader.SetGlobalVector(this.playerViewportPositionProperty, this.currentCamera.WorldToViewportPoint(this.Player.transform.position));
        else
            Shader.SetGlobalVector(this.playerViewportPositionProperty, Vector3.zero);
    }
}
