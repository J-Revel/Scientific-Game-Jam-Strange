using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleCounterGauge : MonoBehaviour
{
    public RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Update()
    {
        rectTransform.anchorMax = new Vector2(ParticleCounter.instance.chargeRatio, 1);
    }
}
