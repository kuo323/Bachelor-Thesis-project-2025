using UnityEngine;

public class RotationGainTrigger : MonoBehaviour
{
    public RotationGainController gainController;
    

    private void OnTriggerEnter(Collider other)
    {


        if (!other.CompareTag("MainCamera")) return;

        if (!gainController.isRedirecting)
        {
            gainController.StartRedirection();
            Debug.Log("➡ User entered trigger zone: rotation gain started.");
        }
    }


}
