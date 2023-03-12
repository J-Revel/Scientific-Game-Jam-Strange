using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleWall : MonoBehaviour
{
    private new SphereCollider collider;
    public int destroyCount = 0;
    public float particleRadius = 0.1f;

    private void Start()
    {
        collider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        for(int i=ParticlePhysicsManager.instance.particleElements.Count-1; i >= 0; i--)
        {
            Vector3 position = ParticlePhysicsManager.instance.particleElements[i].transform.position;
            Vector3 center = collider.transform.position + collider.center * transform.lossyScale.sqrMagnitude; 
            Vector3 offset = position - center;
            float scale = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3;
            if(offset.sqrMagnitude <= (collider.radius + particleRadius) * (collider.radius + particleRadius) * scale * scale)
            {
                Vector3 d = position - center;
                Vector2 normal = new Vector2(d.x, d.z).normalized;
                Vector2 tangent = new Vector2(normal.y, -normal.x);
                float normalSpeed = Vector2.Dot(ParticlePhysicsManager.instance.particleVelocities[i], normal);
                float tangentSpeed = Vector2.Dot(ParticlePhysicsManager.instance.particleVelocities[i], tangent);
                ParticlePhysicsManager.instance.particleVelocities[i] = tangentSpeed * tangent + normal * Mathf.Abs(normalSpeed);
                destroyCount++;
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
