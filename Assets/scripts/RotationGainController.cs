using UnityEngine;

public class RotationGainController : MonoBehaviour
{
    [Header("References")]
    public Transform rig;        // The XR Rig or player root
    public Transform head;       // The camera (OVR/Meta XR camera)

    [Header("Settings")]
    public float gainMultiplier = 1.5f;  // 1.05 = 5% rotation gain
    public RoomBounderiesChecker boundaryChecker;

    private float lastHeadYaw;

    void Start()
    {
        if (head == null)
            Debug.LogError("Head Transform not assigned!");

        // Record initial yaw angle
        lastHeadYaw = head.eulerAngles.y;
    }

    void Update()
    {
        // Only apply gain if player is outside boundary
        if (boundaryChecker != null && boundaryChecker.IsOutside())
        {
            ApplyRotationGain();
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
