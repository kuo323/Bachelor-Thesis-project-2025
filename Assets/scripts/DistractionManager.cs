using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class DistractionManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject orbClusterPrefab; // empty cluster prefab
    public GameObject orbPrefab;        // your custom orb prefab

    [Header("Player Reference")]
    private Transform head;

    [Header("Cluster Shape")]
    public Vector3 clusterSize = new Vector3(2f, 1f, 2f); // width, height, depth

    [Header("Cluster Settings")]
    public int minOrbsPerCluster = 3;
    public int maxOrbsPerCluster = 6;

    private GameObject currentCluster;
    private Vector3 orbitCenter;

    [Header("Orbit Settings")]
    public float rotationSpeed = 10f;        // degrees per second
    public float followSpeed = 2f;           // how fast orbit center follows player

    [Header("Distance Oscillation")]
    public float minDistance = 0.3f;
    public float maxDistance = 1.0f;
    public float distanceOscillationSpeed = 1f;  // controls how fast distance oscillates

    void Start()
    {
        head = CameraManager.Instance.head;
    }

    void Update()
    {
        if (currentCluster != null)
        {
            RotateCluster();
            UpdateOrbitDistance();
        }
    }

    public void SpawnCluster()
    {
        // Initial spawn in front of player
        Vector3 forwardOffset = head.forward * Random.Range(minDistance, maxDistance);
        Vector3 randomOffset = new Vector3(
            Random.Range(-0.5f, 0.5f),
            0f, // no vertical offset
            Random.Range(-0.5f, 0.5f)
        );

        Vector3 spawnPos = head.position + forwardOffset + randomOffset;
        spawnPos.y = 1.65f; // fixed human head height

        currentCluster = Instantiate(orbClusterPrefab, spawnPos, Quaternion.identity);

        orbitCenter = head.position;

        // Spawn orbs inside cluster
        int orbCount = Random.Range(minOrbsPerCluster, maxOrbsPerCluster + 1);
        for (int j = 0; j < orbCount; j++)
        {
            GameObject orb = Instantiate(orbPrefab, currentCluster.transform);
            orb.transform.localPosition = new Vector3(
                Random.Range(-clusterSize.x / 2f, clusterSize.x / 2f),
                Random.Range(-clusterSize.y / 2f, clusterSize.y / 2f),
                Random.Range(-clusterSize.z / 2f, clusterSize.z / 2f)
            );

            orb.AddComponent<OrbDrift>();
            orb.AddComponent<OrbAbsorb>();
        }
    }

    private void RotateCluster()
    {
        // Rotate cluster around orbit center
        currentCluster.transform.RotateAround(orbitCenter, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void UpdateOrbitDistance()
    {
        // Smooth distance oscillation between minDistance and maxDistance
        float targetDistance = Mathf.Lerp(minDistance, maxDistance,
            (Mathf.Sin(Time.time * distanceOscillationSpeed) + 1f) / 2f);

        // Current direction from player to cluster (horizontal plane)
        Vector3 direction = (currentCluster.transform.position - head.position).normalized;

        // Smoothly interpolate distance toward targetDistance
        float currentDistance = Vector3.Distance(currentCluster.transform.position, head.position);
        float newDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * followSpeed);

        // Update cluster position (keep fixed vertical height)
        Vector3 newPos = head.position + direction * newDistance;
        newPos.y = 1.65f; // fixed head height
        currentCluster.transform.position = newPos;

        // Slowly move orbit center toward player
        orbitCenter = Vector3.Lerp(orbitCenter, head.position, Time.deltaTime * followSpeed);
    }
}

