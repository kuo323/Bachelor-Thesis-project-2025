using UnityEngine;

public class DistractionManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject orbClusterPrefab; // empty cluster prefab
    public GameObject orbPrefab;        // your custom orb prefab

    [Header("Player Reference")]
    public Transform playerHead;

    [Header("Cluster Settings")]
    public int minOrbsPerCluster = 3;
    public int maxOrbsPerCluster = 6;
    public int totalClusters = 3;
    public float spawnRadius = 1.5f; // distance from player

    void Start()
    {
        for (int i = 0; i < totalClusters; i++)
        {
            SpawnCluster();
        }
    }

    void SpawnCluster()
    {
        // Random position around player
        Vector3 spawnPos = playerHead.position + Random.insideUnitSphere * spawnRadius;
        spawnPos.y = playerHead.position.y; // keep at head height

        // Instantiate cluster
        GameObject cluster = Instantiate(orbClusterPrefab, spawnPos, Quaternion.identity);

        // Random number of orbs
        int orbCount = Random.Range(minOrbsPerCluster, maxOrbsPerCluster + 1);

        for (int j = 0; j < orbCount; j++)
        {
            // Instantiate your custom orb prefab
            GameObject orb = Instantiate(orbPrefab, cluster.transform);

            // Random local position inside the cluster
            orb.transform.localPosition = Random.insideUnitSphere * 4f;

            // Add drift and absorption scripts
            orb.AddComponent<OrbDrift>();
            orb.AddComponent<OrbAbsorb>();
        }
    }
}
