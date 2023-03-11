using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleSpawner : MonoBehaviour
{
    public float spawnRadius = 10;
    public float minHoleRadius = 0.5f, maxHoleRadius = 2;
    public ScaleInOut holePrefab;
    public float spawnInterval = 3;
    IEnumerator Start()
    {
        while(true)
        {
            yield return new WaitForSeconds(spawnInterval);
            float angle = Random.Range(0, Mathf.PI * 2);
            float distance = Random.Range(0, spawnRadius);
            ScaleInOut hole = Instantiate(holePrefab, transform.position + distance * (Vector3.right * Mathf.Cos(angle) + Vector3.forward * Mathf.Sin(angle)), Quaternion.identity);
            hole.targetScale = Random.Range(minHoleRadius, maxHoleRadius);
        }
    }
}
