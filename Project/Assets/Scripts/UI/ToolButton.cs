using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolButton : MonoBehaviour
{
    public int toolIndex;
    public GameObject selectedDisplay;
    
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { ToolSelector.instance.SelectTool(toolIndex);});
    }

    void Update()
    {
        selectedDisplay.SetActive(ToolSelector.instance.selectedTool == toolIndex);
    }
}
