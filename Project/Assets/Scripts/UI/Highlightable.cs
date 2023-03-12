using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Highlightable : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Material highlightMaterial;
    private Material defaultMaterial;


    
    public void Start()
    {
        defaultMaterial = meshRenderer.material;
    }

    public void SetHighlighted(bool highlighted)
    {
        meshRenderer.material = highlighted ? highlightMaterial : defaultMaterial;
    }
}
