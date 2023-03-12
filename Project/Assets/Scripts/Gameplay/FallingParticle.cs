using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingParticle : MonoBehaviour
{
    public float gravity = 10;
    public Vector3 velocity;
    public float lifeTime = 5;
    void Start()
    {
        
    }

    void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        transform.position += velocity * Time.fixedDeltaTime;
        velocity += gravity * Vector3.down * Time.fixedDeltaTime;
    }
}
