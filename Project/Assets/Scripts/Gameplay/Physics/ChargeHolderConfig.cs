using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FieldType {
    None,
    Electric,
    Magnetic,
}

[CreateAssetMenu()]
public class ChargeHolderConfig : ScriptableObject
{
    public FieldType fieldType;
    public float charge = 10;
    public float range = 5;
}
