using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeHolder : MonoBehaviour
{
    public ChargeHolderConfig config;
    public FieldType fieldType { get { return config.fieldType; } }
    public bool active = true;
    public float displayCharge = 0;
    public float appearDisappearDuration = 1;
    public Transform displayTransform;
    public Material positiveChargeMaterial;
    public Material negativeChargeMaterial;
    public bool negativeCharge = false;

    public float range { get { return config.range; }}
    public float charge { get { return config.charge * (negativeCharge ? -1 : 1); } }

    private IEnumerator Start()
    {
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            if(positiveChargeMaterial != null && negativeChargeMaterial != null)
                renderer.material = negativeCharge ? negativeChargeMaterial : positiveChargeMaterial;
        }
        switch(config.fieldType)
        {
            case FieldType.Electric:
                ParticlePhysicsManager.instance.chargePositions.Add(this);
                break;
            case FieldType.Magnetic:
                ParticlePhysicsManager.instance.magneticPositions.Add(this);
                break;
        }
        for(float time=0; time < appearDisappearDuration; time += Time.deltaTime)
        {
            float f = time / appearDisappearDuration;
            f = 1 - (1-f) * (1-f);
            if(displayTransform != null)
                displayTransform.localScale = Vector3.one * f;
            displayCharge = config.charge * f * (negativeCharge ? -1 : 1);
            yield return null;
        }

        while(active)
        {
            displayCharge = config.charge * (negativeCharge ? -1 : 1);
            yield return null;
        }

        for(float time = 0; time < appearDisappearDuration; time += Time.deltaTime)
        {
            float f = time / appearDisappearDuration;
            f = 1 - (1-f) * (1-f);
            if(displayTransform != null)
                displayTransform.localScale = Vector3.one * (1 - f);
            displayCharge = config.charge * (1 - f) * (negativeCharge ? -1 : 1);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        switch(config.fieldType)
        {
            case FieldType.Electric:
                ParticlePhysicsManager.instance.chargePositions.Remove(this);
                break;
            case FieldType.Magnetic:
                ParticlePhysicsManager.instance.magneticPositions.Remove(this);
                break;
        }
    }
}
