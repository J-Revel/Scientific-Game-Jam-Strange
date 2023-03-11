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
    public float arrowScale = 1;
    public float maxArrowScale = 3;
    public float forceIntensities = 1;

    void Awake()
    {
        instance = this;
    }

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
            for(int j=0; j<chargePositions.Count; j++)
            {
                Vector2 direction = new Vector2(chargePositions[j].transform.position.x, chargePositions[j].transform.position.z) - cellPosition;
                float r2 = direction.sqrMagnitude;
                electricField[i] += chargePositions[j].charge / r2 * direction.normalized;
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
            // float x = (float)particlePositions[i].x / cellSize;
            // float y = (float)particlePositions[i].y / cellSize;
            // int x1 = Mathf.Clamp(Mathf.FloorToInt(x), 0, cellCount.x - 1);
            // int x2 = Mathf.Clamp(Mathf.CeilToInt(x), 0, cellCount.x - 1);
            // int y1 = Mathf.Clamp(Mathf.FloorToInt(y), 0, cellCount.y - 1);
            // int y2 = Mathf.Clamp(Mathf.CeilToInt(y), 0, cellCount.y - 1);

            // Vector2 chargex1y1 = electricField[x1 + y1 * cellCount.x];
            // Vector2 chargex2y1 = electricField[x2 + y1 * cellCount.x];

            // Vector2 chargey1 = Vector2.Lerp(chargex1y1, chargex2y1, (x - x1) / (x2 - x1));
            // if(x2 == x1)
            //     chargey1 = (chargex1y1 + chargex2y1)/2;

            // Vector2 chargex1y2 = electricField[x1 + y2 * cellCount.x];
            // Vector2 chargex2y2 = electricField[x2 + y2 * cellCount.x];

            // Vector2 chargey2 = Vector2.Lerp(chargex1y2, chargex2y2, (x - x1) / (x2 - x1));
            // if(x2 == x1)
            //     chargey2 = (chargex1y2 + chargex2y2)/2;

            // Vector2 chargeAverage = Vector2.Lerp(chargey1, chargey2, (y - y1) / (y2 - y1));
            // if(y2 == y1)
            //     chargeAverage = (chargey2 + chargey1)/2;

            for(int j=0; j<chargePositions.Count; j++)
            {
                Vector2 direction = new Vector2(chargePositions[j].transform.position.x, chargePositions[j].transform.position.z) - particlePositions[i];
                float r2 = direction.sqrMagnitude;
                particleVelocities[i] += forceIntensities * chargePositions[j].charge / r2 * direction.normalized * Time.fixedDeltaTime;
            }

            // particleVelocities[i] += forceIntensities * chargeAverage * Time.fixedDeltaTime;
            particlePositions[i] += particleVelocities[i] * Time.fixedDeltaTime;
            particleElements[i].transform.position = new Vector3(particlePositions[i].x, 0, particlePositions[i].y);
        }
    }
}
