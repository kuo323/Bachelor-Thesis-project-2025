using UnityEngine;

public class RotationGainTrigger : MonoBehaviour
{
    public RotationGainController gainController;
    public DistractionManager distractionManager;

    private bool clusterSpawnedOnce = false;

    private void OnTriggerEnter(Collider other)
    {


        if (!other.CompareTag("MainCamera")) return;

        if (!gainController.isRedirecting && !clusterSpawnedOnce)
        {
            gainController.StartRedirection();
            distractionManager.SpawnCluster();
            clusterSpawnedOnce = true;

            Debug.Log("➡ User entered trigger zone: rotation gain started & cluster spawned once.");
        }

    }


}
