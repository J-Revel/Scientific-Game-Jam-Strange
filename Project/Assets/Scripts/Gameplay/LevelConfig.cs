using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConfig : MonoBehaviour
{
    public static LevelConfig instance;
    public ExpectedCharge particleCharges;
    public bool[] allowedTools = new bool[4];

    private void Awake()
    {
        instance = this;
    }
}
