using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FieldType {
    None,
    Electric,
    Magnetic,
}

public class ChargeHolder : MonoBehaviour
{
    public FieldType fieldType;
    public bool active = true;
    public float charge = 10;
    public float range = 5;
    public float displayCharge = 0;
    public float appearDisappearDuration = 1;
    public Transform displayTransform;
    public Material positiveChargeMaterial;
    public Material negativeChargeMaterial;

    private IEnumerator Start()
    {
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.material = charge > 0 ? positiveChargeMaterial : negativeChargeMaterial;
        }
        switch(fieldType)
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
            displayCharge = charge * f;
            yield return null;
        }

        while(active)
            yield return null;

        for(float time = 0; time < appearDisappearDuration; time += Time.deltaTime)
        {
            float f = time / appearDisappearDuration;
            f = 1 - (1-f) * (1-f);
            if(displayTransform != null)
                displayTransform.localScale = Vector3.one * (1 - f);
            displayCharge = charge * (1 - f);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        switch(fieldType)
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
