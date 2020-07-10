using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] LevelUI levelUI;


    public LevelUI LevelUI => this.levelUI;
}
