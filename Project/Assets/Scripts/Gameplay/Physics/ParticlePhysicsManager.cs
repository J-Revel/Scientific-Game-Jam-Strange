using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePhysicsManager : MonoBehaviour
{
    public static ParticlePhysicsManager instance;
    public float cellSize = 1;
    public Vector2Int cellCount = new Vector2Int(10, 10);
    [System.NonSerialized]

    private Transform[] electricFieldArrows;
    public Transform electricFieldArrowPrefab;
    [System.NonSerialized]

    public Vector2[] electricField;
    
    [System.NonSerialized]

    private Transform[] magneticFieldArrows;
    public Transform magneticFieldArrowPrefab;
    [System.NonSerialized]

    public Vector3[] magneticField;
    
    [System.NonSerialized]
    public List<Vector2> particlePositions = new List<Vector2>();
    [System.NonSerialized]
    public List<Vector2> particleVelocities = new List<Vector2>();
    [System.NonSerialized]
    public List<ChargeHolder> particleElements = new List<ChargeHolder>();
    [System.NonSerialized]
    public List<ChargeHolder> chargePositions = new List<ChargeHolder>();
    [System.NonSerialized]
    public List<ChargeHolder> magneticPositions = new List<ChargeHolder>();
    public float arrowScale = 1;
    public float magneticArrowScale = 1;
    public float maxArrowScale = 3;
    public float maxMagneticArrowScale = 3;
    public float forceIntensities = 1;
    public float magneticIntensities = 1;
    private float rotTime = 0;
    public float rotationSpeed = 1;
    public float maxForce = 10;
    public float minForce = 0.01f;

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
        rotTime += Time.deltaTime;
        UpdateFields();
    }
    public Vector2 ElectricFieldVector(Vector2 point)
    {
        Vector2 result = Vector2.zero;
        for(int j=0; j<chargePositions.Count; j++)
        {
            Vector2 direction = new Vector2(chargePositions[j].transform.position.x, chargePositions[j].transform.position.z) - point;
            float r = Mathf.Clamp01(direction.magnitude / chargePositions[j].range);
            result += chargePositions[j].displayCharge * (1-r) * (1-r) * direction.normalized;
        }
        return result;
    }
    
    public Vector3 MagneticFieldVector(Vector2 point)
    {
        Vector3 result = Vector2.zero;
        for(int j=0; j<magneticPositions.Count; j++)
        {
            Vector2 direction = new Vector2(magneticPositions[j].transform.position.x, magneticPositions[j].transform.position.z) - point;
            float r = Mathf.Clamp01(direction.magnitude / magneticPositions[j].range);
            result += magneticPositions[j].displayCharge * (1-r) * (1-r) * Vector3.up;
        }
        return result;
    }

    private void UpdateFields()
    {
        for(int i=0; i<electricField.Length; i++)
        {
            electricField[i] = Vector3.zero;
            Vector2 cellPosition = cellSize * new Vector2(i%cellCount.x, i/cellCount.y);
            electricField[i] += ElectricFieldVector(cellPosition);
            // for(int j=0; j<chargePositions.Count; j++)
            // {
            //     Vector2 direction = new Vector2(chargePositions[j].transform.position.x, chargePositions[j].transform.position.z) - cellPosition;
            //     float r = direction.magnitude / chargePositions[j].range;
            //      += chargePositions[j].displayCharge * (1-r)*(1-r) * direction.normalized;
            // }
        }
        for(int i=0; i<magneticField.Length; i++)
        {
            magneticField[i] = Vector3.zero;
            Vector2 cellPosition = cellSize * new Vector2(i%cellCount.x, i/cellCount.y);
            magneticField[i] += MagneticFieldVector(cellPosition);
            // for(int j=0; j<magneticPositions.Count; j++)
            // {
            //     Vector2 direction = new Vector2(magneticPositions[j].transform.position.x, magneticPositions[j].transform.position.z) - cellPosition;
            //     float r = direction.magnitude;
            //     magneticField[i] += magneticPositions[j].displayCharge / r * magneticPositions[j].transform.up;
            // }
        }
        for(int i=0; i<magneticField.Length; i++)
        {
            if(magneticField[i] != Vector3.zero)
            {
                magneticFieldArrows[i].rotation = Quaternion.AngleAxis(rotTime * rotationSpeed, magneticField[i]) * Quaternion.LookRotation(magneticField[i]);
                float force = magneticField[i].magnitude;
                magneticFieldArrows[i].localScale = Vector3.one * Mathf.Lerp(0, 1, (force - minForce) / (maxForce - minForce)) * magneticArrowScale;
            }
            else
            {
                magneticFieldArrows[i].localScale = Vector3.zero;
            }
        }
        for(int i=0; i<electricField.Length; i++)
        {
            Vector3 dir = new Vector3(electricField[i].x, 0, electricField[i].y);
            if(dir != Vector3.zero)
                electricFieldArrows[i].rotation = Quaternion.LookRotation(dir);
            float force = electricField[i].magnitude;
            electricFieldArrows[i].localScale = Vector3.one * Mathf.Lerp(0, 1, (force - minForce) / (maxForce - minForce))  * arrowScale;
        }
    }

    void FixedUpdate()
    {
        for(int i=0; i<particlePositions.Count; i++)
        {
            Vector3 magneticFieldForce = Vector3.Cross(new Vector3(particleVelocities[i].x, 0, particleVelocities[i].y), MagneticFieldVector(particlePositions[i]));
            Vector2 electricFieldForce = ElectricFieldVector(particlePositions[i]);
            particleVelocities[i] += particleElements[i].charge * (electricFieldForce + new Vector2(magneticFieldForce.x, magneticFieldForce.z)) * Time.fixedDeltaTime;
            // for(int j=0; j<chargePositions.Count; j++)
            // {
            //     Vector2 direction = new Vector2(chargePositions[j].transform.position.x, chargePositions[j].transform.position.z) - particlePositions[i];
            //     float r2 = direction.sqrMagnitude;
            //     Vector2 force = forceIntensities * chargePositions[j].displayCharge * particleElements[i].charge / r2 * direction.normalized * Time.fixedDeltaTime;
                
            //     force = force.normalized * Mathf.Lerp(0, maxForce, (force.magnitude - minForce) / (maxForce - minForce));
            //     particleVelocities[i] += force;
            // }
            // for(int j=0; j<magneticPositions.Count; j++)
            // {
            //     Vector2 direction = new Vector2(magneticPositions[j].transform.position.x, magneticPositions[j].transform.position.z) - particlePositions[i];
            //     float r2 = direction.sqrMagnitude;
            //     Vector3 force = magneticIntensities * Vector3.Cross(magneticPositions[j].displayCharge * particleElements[i].charge / r2 * magneticPositions[j].transform.up, new Vector3(particleVelocities[i].x, 0, particleVelocities[i].y));
            //     force =  force.normalized * Mathf.Lerp(0, maxForce, (force.magnitude - minForce) / (maxForce - minForce));
            //     particleVelocities[i] += new Vector2(force.x, force.z);
            // }

            particlePositions[i] += particleVelocities[i] * Time.fixedDeltaTime;
            particleElements[i].transform.position = new Vector3(particlePositions[i].x, 0, particlePositions[i].y);
        }
    }
}
