using System.Collections;
using UnityEngine;


public class DistractionManager : MonoBehaviour
{

    public RotationGainController rotationGainController; // assign in Inspector

    [Header("Prefabs")]
    public GameObject orbClusterPrefab; // empty cluster prefab
    public GameObject orbPrefab;        // orb prefab

    [Header("Player Reference")]
    private Transform head;

    [Header("Cluster Shape")]
    public Vector3 clusterSize = new Vector3(2f, 1f, 2f);

    [Header("Cluster Settings")]
    public int minOrbsPerCluster = 3;
    public int maxOrbsPerCluster = 6;

    private GameObject currentCluster;
    private Vector3 orbitCenter;

    [Header("Orbit Settings")]
    public float rotationSpeed = 20f;           // degrees per second
    public float followSpeed = 2f;              // orbit center follow speed
    public bool isHit = false;
    public float rotationSpeedAfterHit = 40f;
    public float afterHitSpeedDuration = 5f;

    private float angle = 0f;                   // current rotation angle
    private float rotationDir = 1f;
    private float orbitRadius;


    private float spawnSmoothTime = 1f; // seconds to ignore orbit distance updates
    private float spawnTimer = 0f;        // countdown timer



    [Header("Orbit timer")]
    private float directionSwitchTimer = 0f;
    public float minSwitchTime = 0.5f;
    public float maxSwitchTime = 2f;



    [Header("Distance Oscillation")]
    public float minDistance = 0.3f;
    public float maxDistance = 1.0f;
    public float distanceOscillationSpeed = 1f; // speed of distance oscillation

    [Header("Cluster Move to Center")]
    public Vector3 roomCenter = Vector3.zero;
    public float moveToCenterSpeed = 1.5f;
    private float centerFloatingHeight = 1.65f;

    private int totalOrbs = 0;
    private int absorbedOrbs = 0;
    private bool movingToCenter = false;

    void Start()
    {
        head = CameraManager.Instance.head;
        ResetOrbitBehavior();
    }

    void Update()
    {
        if (currentCluster != null)
        {
            if (!movingToCenter)
            {
                UpdateOrbitBehavior();
                RotateCluster();

                if (spawnTimer <= 0f)
                {
                    UpdateOrbitDistance();
                }
                else
                {
                    spawnTimer -= Time.deltaTime;
                }
            }
            else
            {
                Vector3 centerPos = new Vector3(roomCenter.x, centerFloatingHeight, roomCenter.z);
                currentCluster.transform.position = Vector3.Lerp(
                    currentCluster.transform.position,
                    centerPos,
                    Time.deltaTime * moveToCenterSpeed
                );
            }
        }

        if (isHit && afterHitSpeedDuration > 0)
        {
            rotationSpeed = rotationSpeedAfterHit;
            afterHitSpeedDuration -= Time.deltaTime;
        }
    }

    // ------------------------
    //    FLY-LIKE BEHAVIOR
    // ------------------------

    private void ResetOrbitBehavior()
    {
        directionSwitchTimer = Random.Range(minSwitchTime, maxSwitchTime);
        rotationDir = Random.value > 0.5f ? 1f : -1f;
        rotationSpeed = Random.Range(12f, 28f);
    }

    private void UpdateOrbitBehavior()
    {
        directionSwitchTimer -= Time.deltaTime;

        if (directionSwitchTimer <= 0f)
        {
            rotationDir = Random.value > 0.5f ? 1f : -1f; // switch direction
            rotationSpeed = Random.Range(12f, 28f);      // new random speed

            directionSwitchTimer = Random.Range(minSwitchTime, maxSwitchTime);
        }
    }

    // ------------------------
    //       EXISTING LOGIC
    // ------------------------

    public void SpawnCluster()
    {


      

        Vector3 forwardOffset = head.forward * maxDistance;
        Vector3 spawnPos = head.position + forwardOffset;
        spawnPos.y = centerFloatingHeight;

        currentCluster = Instantiate(orbClusterPrefab, spawnPos, Quaternion.identity);
        orbitCenter = head.position;

        orbitRadius = Vector3.Distance(currentCluster.transform.position, head.position);


        spawnTimer = spawnSmoothTime;



        int orbCount = Random.Range(minOrbsPerCluster, maxOrbsPerCluster + 1);
        totalOrbs = orbCount;
        absorbedOrbs = 0;
        movingToCenter = false;

        ResetOrbitBehavior();

        for (int j = 0; j < orbCount; j++)
        {
            GameObject orb = Instantiate(orbPrefab, currentCluster.transform);
            orb.transform.localPosition = new Vector3(
                Random.Range(-clusterSize.x / 2f, clusterSize.x / 2f),
                Random.Range(-clusterSize.y / 2f, clusterSize.y / 2f),
                Random.Range(-clusterSize.z / 2f, clusterSize.z / 2f)
            );

            OrbDrift drift = orb.AddComponent<OrbDrift>();
            OrbAbsorb absorb = orb.AddComponent<OrbAbsorb>();

            absorb.rotationGainController = rotationGainController;
            absorb.distractionManager = this;
        }
    }

    private void RotateCluster()
    {
        if (currentCluster == null) return;

        angle += rotationDir * rotationSpeed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;

        float radius = orbitRadius;

        Vector3 center = orbitCenter;

        float x = center.x + Mathf.Cos(rad) * radius;
        float z = center.z + Mathf.Sin(rad) * radius;

        float y = currentCluster.transform.position.y;

        currentCluster.transform.position = new Vector3(x, y, z);
    }

    private void UpdateOrbitDistance()
    {
        if (currentCluster == null) return;

        float oscillation = (Mathf.Sin(Time.time * distanceOscillationSpeed) + 1f) / 2f;
        float targetDistance = Mathf.Lerp(minDistance, maxDistance, oscillation);

        Vector3 direction = (currentCluster.transform.position - head.position).normalized;
        Vector3 newPos = head.position + direction * targetDistance;

        float floatOffset = Mathf.Sin(Time.time * 1.5f) * 0.2f;
        newPos.y = centerFloatingHeight + floatOffset;

        currentCluster.transform.position = newPos;

        orbitCenter = Vector3.Lerp(orbitCenter, head.position, Time.deltaTime * followSpeed);
    }

    public void afterHit() { isHit = true; }

    public void OrbAbsorbed()
    {
        absorbedOrbs++;
        if (!movingToCenter && absorbedOrbs >= Mathf.CeilToInt(totalOrbs * 2f / 3f))
        {
            movingToCenter = true;
        }
    }

}

