using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePhysicsManager : MonoBehaviour
{
    public static ParticlePhysicsManager instance;
    public float cellSize = 1;
    public Vector2Int cellCount = new Vector2Int(10, 10);
    private Transform[] electricFieldArrows;
    public Transform electricFieldArrowPrefab;
    public Vector2[] electricField;
    
    private Transform[] magneticFieldArrows;
    public Transform magneticFieldArrowPrefab;
    public Vector3[] magneticField;
    
    public List<Vector2> particlePositions = new List<Vector2>();
    public List<Vector2> particleVelocities = new List<Vector2>();
    public List<ChargeHolder> particleElements = new List<ChargeHolder>();
    public List<ChargeHolder> chargePositions = new List<ChargeHolder>();
    public List<ChargeHolder> magneticPositions = new List<ChargeHolder>();
    public float arrowScale = 1;
    public float maxArrowScale = 3;
    public float forceIntensities = 1;
    public float magneticIntensities = 1;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        electricField = new Vector2[cellCount.x * cellCount.y];
        magneticField = new Vector3[cellCount.x * cellCount.y];
        electricFieldArrows = new Transform[cellCount.x * cellCount.y];
        magneticFieldArrows = new Transform[cellCount.x * cellCount.y];
        for(int i=0; i<cellCount.x * cellCount.y; i++)
        {
            int x = i%cellCount.x;
            int y = i/cellCount.x;
            electricFieldArrows[i] = Instantiate(electricFieldArrowPrefab, cellSize * (x * Vector3.right + y * Vector3.forward), Quaternion.identity);
            magneticFieldArrows[i] = Instantiate(magneticFieldArrowPrefab, cellSize * (x * Vector3.right + y * Vector3.forward), Quaternion.identity);
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
            for(int j=0; j<chargePositions.Count; j++)
            {
                Vector2 direction = new Vector2(chargePositions[j].transform.position.x, chargePositions[j].transform.position.z) - cellPosition;
                float r2 = direction.sqrMagnitude;
                electricField[i] += chargePositions[j].charge / r2 * direction.normalized;
            }
        }
        for(int i=0; i<magneticField.Length; i++)
        {
            magneticField[i] = Vector3.zero;
            Vector2 cellPosition = cellSize * new Vector2(i%cellCount.x, i/cellCount.y);
            for(int j=0; j<magneticPositions.Count; j++)
            {
                Vector2 direction = new Vector2(magneticPositions[j].transform.position.x, magneticPositions[j].transform.position.z) - cellPosition;
                float r = direction.magnitude;
                magneticField[i] += magneticPositions[j].charge / r * magneticPositions[j].transform.up;
            }
        }
        for(int i=0; i<magneticField.Length; i++)
        {
            magneticFieldArrows[i].rotation = Quaternion.LookRotation(magneticField[i]);
            magneticFieldArrows[i].localScale = Vector3.one * Mathf.Min(magneticField[i].magnitude * arrowScale, maxArrowScale);
        }
        for(int i=0; i<electricField.Length; i++)
        {
            Vector3 dir = new Vector3(electricField[i].x, 0, electricField[i].y);
            if(dir != Vector3.zero)
                electricFieldArrows[i].rotation = Quaternion.LookRotation(dir);
            electricFieldArrows[i].localScale = Vector3.one * Mathf.Min(electricField[i].magnitude * arrowScale, maxArrowScale);
        }
    }

    void FixedUpdate()
    {
        for(int i=0; i<particlePositions.Count; i++)
        {
            for(int j=0; j<chargePositions.Count; j++)
            {
                Vector2 direction = new Vector2(chargePositions[j].transform.position.x, chargePositions[j].transform.position.z) - particlePositions[i];
                float r2 = direction.sqrMagnitude;
                particleVelocities[i] += forceIntensities * chargePositions[j].charge * particleElements[i].charge / r2 * direction.normalized * Time.fixedDeltaTime;
            }
            for(int j=0; j<magneticPositions.Count; j++)
            {
                Vector2 direction = new Vector2(magneticPositions[j].transform.position.x, magneticPositions[j].transform.position.z) - particlePositions[i];
                float r = direction.magnitude;
                Vector3 force = Vector3.Cross(magneticPositions[j].charge * particleElements[i].charge / r * magneticPositions[j].transform.up, new Vector3(particleVelocities[i].x, 0, particleVelocities[i].y));
                particleVelocities[i] += new Vector2(force.x, force.z) * magneticIntensities;
            }

            particlePositions[i] += particleVelocities[i] * Time.fixedDeltaTime;
            particleElements[i].transform.position = new Vector3(particlePositions[i].x, 0, particlePositions[i].y);
        }
    }
}
