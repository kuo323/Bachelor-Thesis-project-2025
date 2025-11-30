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

    [Header("Respawn Settings")]
    public float respawnDelay = 1.5f;   // wait before starting a new cluster
    public float orbSpawnInterval = 0.4f; // delay between orbs
    public bool allowRespawn = true;   // if false, nothing respawns
    private Coroutine gradualRespawnRoutine;


    public bool isHit = false;
    public float rotationSpeedAfterHit = 40f;
    public float afterHitSpeedDuration = 5f;

    private float angle = 0f;                   // current rotation angle
    private float rotationDir = 1f;
    private float orbitRadius;



    [Header("Orbit timer")]
    private float directionSwitchTimer = 0f;
    public float minSwitchTime = 0.5f;
    public float maxSwitchTime = 2f;



    [Header("Distance Oscillation")]
    public float minDistance = 0.3f;
    public float maxDistance = 1.0f;
    public float distanceOscillationSpeed = 1f; // speed of distance oscillation

    [Header("Cluster Move to Center")]
   

    private int totalOrbs = 0;
    private int absorbedOrbs = 0;
    private bool panicMode = false;

    void Start()
    {
        head = CameraManager.Instance.head;
        //ResetOrbitBehavior();
    }

    void Update()
    {
        if (currentCluster != null)
        {
            // Always update orbit behavior and movement
            UpdateOrbitBehavior();
            RotateCluster();
            UpdateOrbitDistance();

            // Speed boost after some orbs are absorbed
            if (panicMode)
            {
                rotationSpeed = rotationSpeedAfterHit; // or increase by a fixed amount
                followSpeed = followSpeed + 10f * Time.deltaTime;
            }

            // Optional: temporary speed boost after hitting
            if (isHit && afterHitSpeedDuration > 0)
            {
                rotationSpeed = rotationSpeedAfterHit;
                afterHitSpeedDuration -= Time.deltaTime;
            }
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
        spawnPos.y = head.position.y + 0.30f;

        currentCluster = Instantiate(orbClusterPrefab, spawnPos, Quaternion.identity);
        orbitCenter = head.position;

       // orbitRadius = Vector3.Distance(currentCluster.transform.position, head.position);
        orbitRadius = (minDistance + maxDistance) / 2f;


     



        int orbCount = Random.Range(minOrbsPerCluster, maxOrbsPerCluster + 1);
        totalOrbs = orbCount;
        absorbedOrbs = 0;
        panicMode = false;

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


    /// <summary>
    /// ords fly around the users head 
    /// </summary>
    private void RotateCluster()
    {
        if (currentCluster == null) return;

       



        // Smooth orbit radius update to prevent sudden jumps
        float currentDistance = Vector3.Distance(currentCluster.transform.position, head.position);
        orbitRadius = Mathf.Lerp(orbitRadius, currentDistance, Time.deltaTime * 2f);

        angle += rotationDir * rotationSpeed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;

        Vector3 center = orbitCenter;

        float x = center.x + Mathf.Cos(rad) * orbitRadius;
        float z = center.z + Mathf.Sin(rad) * orbitRadius;
        float y = currentCluster.transform.position.y;

        currentCluster.transform.position = new Vector3(x, y, z);
    }



    /// <summary>
    /// ords fly around the users head with smooth motions 
    /// </summary>
    private void UpdateOrbitDistance()
    {
        if (currentCluster == null) return;

        float oscillation = (Mathf.Sin(Time.time * distanceOscillationSpeed) + 1f) / 2f;
        float targetDistance = Mathf.Lerp(minDistance, maxDistance, oscillation);

        Vector3 direction = (currentCluster.transform.position - head.position).normalized;
        Vector3 newPos = head.position + direction * targetDistance;

        float floatOffset = Mathf.Sin(Time.time * 1.5f) * 0.3f;
        newPos.y = head.position.y + 0.3f + floatOffset;

        currentCluster.transform.position = newPos;

        orbitCenter = Vector3.Lerp(orbitCenter, head.position, Time.deltaTime * followSpeed);





    }

    public void afterHit() { isHit = true; }

    public void OrbAbsorbed()
    {
        absorbedOrbs++;
        if (!panicMode && absorbedOrbs >= Mathf.CeilToInt(totalOrbs * 2f / 3f))
        {
            panicMode = true;
        }

        // If all orbs are gone...
        if (absorbedOrbs >= totalOrbs)
        {
            Destroy(currentCluster);
            currentCluster = null;

            Debug.Log("All orbs absorbed — cluster removed.");

            panicMode = false;
            isHit = false;



            if (rotationGainController != null && rotationGainController.hasRotatedOnce)
            {
                Debug.Log("Rotation goal reached — no more respawns.");
                return;
            }
            else
            {
                // avoid double-starting
                if (gradualRespawnRoutine == null)
                    gradualRespawnRoutine = StartCoroutine(RespawnClusterSlowly());
            }

        }


    }


    /// <summary>
    /// This is the part that spawns the new orbs one by one: if the first cluster all get killed
    /// but 180 reset hasnt reached yet
    /// </summary>
    /// <returns></returns>

    private IEnumerator RespawnClusterSlowly()
    {
        // wait a bit before starting
        yield return new WaitForSeconds(respawnDelay);

        // If rotation goal reached in the meantime, abort
        if (rotationGainController != null && rotationGainController.hasRotatedOnce)
        {
            gradualRespawnRoutine = null;
            yield break;
        }

        // Create a new cluster container at appropriate position
        Vector3 forwardOffset = head.forward * maxDistance;
        Vector3 spawnPos = head.position + forwardOffset;
        spawnPos.y = head.position.y + 0.30f;

        currentCluster = Instantiate(orbClusterPrefab, spawnPos, Quaternion.identity);
        orbitCenter = head.position;
        orbitRadius = (minDistance + maxDistance) / 2f;

        // decide how many orbs to spawn (same as original cluster size)
        int orbCount = Random.Range(minOrbsPerCluster, maxOrbsPerCluster + 1);
        totalOrbs = orbCount;
        absorbedOrbs = 0;
        panicMode = false;

        ResetOrbitBehavior();

        for (int i = 0; i < orbCount; i++)
        {
            // If rotation goal reached mid-spawn, stop spawning
            if (rotationGainController != null && rotationGainController.hasRotatedOnce)
                break;

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

            // small delay between spawns
            yield return new WaitForSeconds(orbSpawnInterval);
        }

        gradualRespawnRoutine = null;
        Debug.Log("Gradual cluster spawn complete.");
    }







}

