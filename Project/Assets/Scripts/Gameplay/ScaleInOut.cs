using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleInOut : MonoBehaviour
{
    public float appearDuration = 5;
    public float stayDuration = 5;
    public float disappearDuration = 5;
    public float targetScale = 1; 
    private float time = 0;

    IEnumerator Start()
    {
        for(float time=0; time < appearDuration; time += Time.deltaTime)
        {
            float f = time / appearDuration;
            transform.localScale = Vector3.one * targetScale * f;
            yield return null;
        }
        yield return new WaitForSeconds(stayDuration);
        for(float time=0; time < appearDuration; time += Time.deltaTime)
        {
            float f = 1 - time / appearDuration;
            transform.localScale = Vector3.one * targetScale * f;
            yield return null;
        }
        Destroy(gameObject);
    }
}
