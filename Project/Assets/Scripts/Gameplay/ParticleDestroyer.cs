using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    private new SphereCollider collider;

    private void Start()
    {
        collider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        for(int i=ParticlePhysicsManager.instance.particleElements.Count-1; i >= 0; i--)
        {
            if(isInside(ParticlePhysicsManager.instance.particleElements[i].transform.position))
            {
                isInside(ParticlePhysicsManager.instance.particleElements[i].transform.position);
                Destroy(ParticlePhysicsManager.instance.particleElements[i].gameObject);
                ParticlePhysicsManager.instance.particleElements.RemoveAt(i);
                ParticlePhysicsManager.instance.particlePositions.RemoveAt(i);
                ParticlePhysicsManager.instance.particleVelocities.RemoveAt(i);
            }
        }
    }

    public bool isInside(Vector3 position)
    {
        Vector3 center = collider.transform.position + collider.center * transform.lossyScale.sqrMagnitude; 
        Vector3 offset = position - center;
        return offset.sqrMagnitude <= collider.radius * collider.radius * (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z)/3;
    }
}
