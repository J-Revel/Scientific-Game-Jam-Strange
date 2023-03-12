using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePhysicsManager : MonoBehaviour
{
    public static ParticlePhysicsManager instance;
    public LayerMask groundRaycastLayer;
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

    public float maxMagneticForce = 10;
    public float minMagneticForce = 0.05f;
    public float arenaRadius = 10;
    public Transform arenaCenter;

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
            RaycastHit hit;
            if(Physics.Raycast(cellSize * (x * Vector3.right + y * Vector3.forward) + Vector3.up, Vector3.down, out hit, 100, groundRaycastLayer))
            {
                electricFieldArrows[i] = Instantiate(electricFieldArrowPrefab, hit.point, Quaternion.identity);
                magneticFieldArrows[i] = Instantiate(magneticFieldArrowPrefab, hit.point, Quaternion.identity);
            }
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
        }
        for(int i=0; i<magneticField.Length; i++)
        {
            magneticField[i] = Vector3.zero;
            Vector2 cellPosition = cellSize * new Vector2(i%cellCount.x, i/cellCount.y);
            magneticField[i] += MagneticFieldVector(cellPosition);
        }

        for(int i=0; i<magneticField.Length; i++)
        {
            if(magneticFieldArrows[i] == null)
                continue;
            if(magneticField[i] != Vector3.zero)
            {
                magneticFieldArrows[i].rotation = Quaternion.AngleAxis(rotTime * rotationSpeed, magneticField[i]) * Quaternion.LookRotation(magneticField[i]);
                float force = magneticField[i].magnitude;
                magneticFieldArrows[i].localScale = Vector3.one * Mathf.Lerp(0, magneticArrowScale, (force - minMagneticForce) / (maxMagneticForce - minMagneticForce));
            }
            else
            {
                magneticFieldArrows[i].localScale = Vector3.zero;
            }
        }
        for(int i=0; i<electricField.Length; i++)
        {
            if(electricFieldArrows[i] == null)
                continue;
            Vector3 dir = new Vector3(electricField[i].x, 0, electricField[i].y);
            if(dir != Vector3.zero)
                electricFieldArrows[i].rotation = Quaternion.LookRotation(dir);
            float force = electricField[i].magnitude;
            electricFieldArrows[i].localScale = Vector3.one * Mathf.Lerp(0, 1, (force - minForce) / (maxForce - minForce))  * arrowScale;
        }
    }

    void FixedUpdate()
    {
        for(int i=particlePositions.Count-1; i>=0; i--)
        {
            Vector3 magneticFieldForce = Vector3.Cross(new Vector3(particleVelocities[i].x, 0, particleVelocities[i].y), MagneticFieldVector(particlePositions[i]));
            Vector2 electricFieldForce = ElectricFieldVector(particlePositions[i]);
            Vector2 newVelocity = particleVelocities[i] + particleElements[i].charge * new Vector2(magneticFieldForce.x, magneticFieldForce.z) * Time.fixedDeltaTime;
            newVelocity = newVelocity.normalized * particleVelocities[i].magnitude;
            particleVelocities[i] = newVelocity + particleElements[i].charge * electricFieldForce * Time.fixedDeltaTime;

            particlePositions[i] += particleVelocities[i] * Time.fixedDeltaTime;
            Vector2 arenaCenterPos = new Vector2(arenaCenter.position.x, arenaCenter.position.z);
            if(!Physics.Raycast(new Vector3(particlePositions[i].x, 0.5f, particlePositions[i].y), Vector3.down, 1, groundRaycastLayer))
            {
                Vector3 pos = new Vector3(particlePositions[i].x, 0.5f, particlePositions[i].y);
                Debug.DrawLine(pos, pos + Vector3.down, Color.white, 3);
                FallingParticle fallingParticle = particleElements[i].gameObject.AddComponent<FallingParticle>();
                fallingParticle.velocity = new Vector3(particleVelocities[i].x, 0, particleVelocities[i].y);
                particlePositions.RemoveAt(i);
                particleVelocities.RemoveAt(i);
                particleElements.RemoveAt(i);

                // Vector2 d = particlePositions[i] - arenaCenterPos;
                // Vector2 normal = d.normalized;
                // Vector2 tangent = new Vector2(normal.y, -normal.x);
                // float normalSpeed = Vector2.Dot(particleVelocities[i], normal);
                // float tangentSpeed = Vector2.Dot(particleVelocities[i], tangent);
                // particleVelocities[i] = tangentSpeed * tangent - normal * Mathf.Abs(normalSpeed);
            }
            else
                particleElements[i].transform.position = new Vector3(particlePositions[i].x, 0, particlePositions[i].y);

            
        }
    }
}
