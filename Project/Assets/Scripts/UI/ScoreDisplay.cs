using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public ParticleDestroyer particleDestroyer;
    public TMPro.TextMeshProUGUI text;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "" + particleDestroyer.destroyCount;
    }
}
