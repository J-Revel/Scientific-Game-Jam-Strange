using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryHandler : MonoBehaviour
{
    public static VictoryHandler instance;
    public GameObject gameOverScreen;
    
    private void Awake()
    {
        instance = this;
    } 

    public void OnVictory()
    {
        Time.timeScale = 0;
        gameOverScreen.SetActive(true);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
