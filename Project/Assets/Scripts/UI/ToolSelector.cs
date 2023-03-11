using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ToolSelector : MonoBehaviour
{
    public static ToolSelector instance;
    public int selectedTool = -1;
    public Transform[] toolSpawnPrefabs;
    public LayerMask raycastLayerMask;

    private void Awake()
    {
        instance = this;
    }

    public void SelectTool(int toolIndex)
    {
        selectedTool = toolIndex;
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
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.zero);
            float enter;
            if(horizontalPlane.Raycast(ray, out enter)) {
                UseTool(ray.GetPoint(enter));
            }
        }
        if(Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100, raycastLayerMask))
            {
                ChargeHolder chargeHolder = hit.collider.GetComponentInParent<ChargeHolder>();
                if(chargeHolder != null)
                {
                    chargeHolder.active = false;
                }
            }
        }
    }
}
