using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolSelector : MonoBehaviour
{
    public static ToolSelector instance;
    public int selectedTool = -1;
    public Transform[] toolSpawnPrefabs;
    public MeshRenderer[] toolHologramPrefabs;
    public Button[] buttons;
    public LayerMask raycastLayerMask;
    public LayerMask groundLayerMask;
    private MeshRenderer toolHologram;
    public Material hologramValidMaterial;
    public Material hologramInvalidMaterial;
    private Highlightable highlighted;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for(int i=0; i<buttons.Length; i++)
        {
            buttons[i].interactable = LevelConfig.instance.allowedTools[i];
        }
    }

    public void SelectTool(int toolIndex)
    {
        selectedTool = toolIndex;
        if(toolHologram != null)
            Destroy(toolHologram.gameObject);
        toolHologram = Instantiate(toolHologramPrefabs[toolIndex], Vector3.zero, Quaternion.identity);
        toolHologram.gameObject.SetActive(false);
    }
    
    public void UseTool(Vector3 position)
    {
        if(selectedTool >= 0)
            Instantiate(toolSpawnPrefabs[selectedTool], position, Quaternion.identity);
    }

    private void Update()
    {
        if(EventSystem.current.IsPointerOverGameObject())
            return;
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100, raycastLayerMask))
            {
                toolHologram.gameObject.SetActive(false);
                Highlightable highlightable = hit.collider.GetComponentInParent<Highlightable>();
                if(highlightable != null)
                {
                    if(highlighted != null)
                    {
                        highlighted.SetHighlighted(false);
                    }
                    highlighted = highlightable;
                    highlighted.SetHighlighted(true);
                }
            }
            else
            {
                if(highlighted != null)
                {
                    highlighted.SetHighlighted(false);
                    highlighted = null;
                }
                if(toolHologram != null)
                {
                    if(Physics.Raycast(ray, out hit, 100, groundLayerMask))
                    {
                        toolHologram.transform.position = hit.point;
                        toolHologram.gameObject.SetActive(true);
                    }
                    else
                        toolHologram.gameObject.SetActive(false);
                }
            }    
        }
        if(Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.zero);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100, raycastLayerMask))
            {
                ChargeHolder chargeHolder = hit.collider.GetComponentInParent<ChargeHolder>();
                if(chargeHolder != null)
                {
                    chargeHolder.active = false;
                }
            }
            else if(Physics.Raycast(ray, out hit, 100, groundLayerMask))
            {
                UseTool(hit.point);
            }
        }
    }
}
