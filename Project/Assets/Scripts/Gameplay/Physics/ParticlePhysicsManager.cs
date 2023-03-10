using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePhysicsManager : MonoBehaviour
{
    public float cellSize = 1;
    public Vector2Int cellCount = new Vector2Int(10, 10);
    private Transform[] electricFieldArrows;
    public Transform electricFieldArrowPrefab;
    public Vector2[] electricField;
    public List<Vector2> particlePositions = new List<Vector2>();
    public List<Vector2> particleVelocities = new List<Vector2>();
    public List<Transform> particleElements = new List<Transform>();
    public List<Transform> chargePositions = new List<Transform>();
    public List<float> charges = new List<float>();
    public float arrowScale = 1;
    public float maxArrowScale = 3;

    void Start()
    {
        electricField = new Vector2[cellCount.x * cellCount.y];
        electricFieldArrows = new Transform[cellCount.x * cellCount.y];
        for(int i=0; i<cellCount.x * cellCount.y; i++)
        {
            int x = i%cellCount.x;
            int y = i/cellCount.x;
            electricFieldArrows[i] = Instantiate(electricFieldArrowPrefab, cellSize * (x * Vector3.right + y * Vector3.forward), Quaternion.identity);
        }
        UpdateFields();
    }

    void Update()
    {
        UpdateFields();
    }

    private void UpdateFields()
    {
        for(int i=0; i<electricField.Length; i++)
        {
            electricField[i] = Vector3.zero;
            Vector2 cellPosition = cellSize * new Vector2(i%cellCount.x, i/cellCount.y);
            for(int j=0; j<charges.Count; j++)
            {
                Vector2 direction = new Vector2(chargePositions[j].position.x, chargePositions[j].position.z) - cellPosition;
                float r2 = direction.sqrMagnitude;
                electricField[i] += charges[j] / r2 * direction.normalized;
            }
        }
        for(int i=0; i<electricField.Length; i++)
        {
            electricFieldArrows[i].rotation = Quaternion.LookRotation(new Vector3(electricField[i].x, 0, electricField[i].y));
            electricFieldArrows[i].localScale = Vector3.one * Mathf.Min(electricField[i].magnitude * arrowScale, maxArrowScale);
        }
    }

    void FixedUpdate()
    {
        for(int i=0; i<particlePositions.Count; i++)
        {
            particleVelocities[i] += electricField[i] * Time.fixedDeltaTime;
            particlePositions[i] += particleVelocities[i] * Time.fixedDeltaTime;
            particleElements[i].position = new Vector3(particlePositions[i].x, 0, particlePositions[i].y);
        }
    }
}
