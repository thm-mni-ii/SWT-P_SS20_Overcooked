using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Canvas uiCanvas;
    [SerializeField] LevelUI levelUI;


    public Canvas UICanvas => this.uiCanvas;
    public LevelUI LevelUI => this.levelUI;
}
