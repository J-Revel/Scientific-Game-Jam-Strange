using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public float spawnInterval = 0.5f;
    public float spawnRadius = 1;
    public float spawnStartSpeed = 1;
    public ChargeHolder particlePrefab;

    private float time = 0;

    private List<ChargeHolder> spawnedParticles = new List<ChargeHolder>();

    void Start()
    {
        
    }

    void Update()
    {
        time += Time.deltaTime;
        if(time > spawnInterval)
        {
            time -= spawnInterval;
            float randomAngle = Random.Range(-Mathf.PI, Mathf.PI);
            float randomRadius = Random.Range(0, spawnRadius);
            ChargeHolder particle = Instantiate(particlePrefab, transform.position + randomRadius * new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)), Quaternion.identity);
            spawnedParticles.Add(particle);
            particle.charge = Random.Range(0, 1.0f) > 0.5f ? -1:1;
            ParticlePhysicsManager.instance.particleElements.Add(particle);
            ParticlePhysicsManager.instance.particlePositions.Add(new Vector2(particle.transform.position.x, particle.transform.position.z));
            ParticlePhysicsManager.instance.particleVelocities.Add(spawnStartSpeed * new Vector2(transform.forward.x, transform.forward.z).normalized);
        }
    }
}
