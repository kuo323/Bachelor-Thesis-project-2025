using UnityEngine;

public class RotationGainController : MonoBehaviour
{

   
    private Transform rig;
    private Transform head;



    [Header("Settings")]
    public float gainMultiplier = 1.5f;  // 1.05 = 5% rotation gain
    public RoomBounderiesChecker boundaryChecker;

    private float lastHeadYaw;

    void Start()
    {
       
        if (CameraManager.Instance == null)
        {
            //make sure the script can get the references from the CameraManager.Instance ///
            Debug.LogError("❌ CameraManager.Instance is null — make sure it's in the scene.");
           // return;
        }

        rig = CameraManager.Instance.rig;
        head = CameraManager.Instance.head;


        if (head == null) Debug.LogError("Head Transform not assigned!");
        if (rig == null) Debug.LogError("Rig Transform not assigned!");

        lastHeadYaw = head.eulerAngles.y;


    }

    void Update()
    {



        // Only apply gain if player is outside boundary
        if (boundaryChecker.IsOutside() && !boundaryChecker.IsFacingCenter())
        {

            
           // ApplyRotationGain();
        }

        // Update last yaw for next frame
        lastHeadYaw = head.eulerAngles.y;
    }

    void ApplyRotationGain()
    {

        Debug.Log("gain is applied");

        float currentYaw = head.eulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(lastHeadYaw, currentYaw);

        // Apply the rotation gain to the *rig*
        float virtualRotation = deltaYaw * (gainMultiplier - 1f);

        rig.Rotate(Vector3.up, virtualRotation);
    }




   

}