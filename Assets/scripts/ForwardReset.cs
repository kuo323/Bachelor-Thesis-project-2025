using UnityEngine;

public class ForwardReset : MonoBehaviour
{
    public Transform rig;   // Your XR Rig root (OVRCameraRig or XR Origin)
    public Transform head;  // VR camera (CenterEyeAnchor)

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch))
        {
            float yaw = head.eulerAngles.y;

            // Rotate XR rig *around the head's position*
            rig.RotateAround(head.position, Vector3.up, -yaw);

            Debug.Log("🔥 Perfect forward reset with no sideways shift!");
        }
    }
}
