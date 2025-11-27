
using System.Collections;

using UnityEngine;

public class RotationGainController : MonoBehaviour
{
    public DistractionManager distractionController;

    public Transform redirectedWorldParent;
    public float rotationGain = 0.1f;

    private Transform head;
    private float lastHeadYaw;


    
    public float targetRotation = 90f;     // total VE rotation goal
    public bool isRedirecting = false;
    private float accumulatedRotation = 0f;

    //single-use flag
    private bool hasRotatedOnce = false;


    void Start()
    {
        head = CameraManager.Instance.head;

        if (head == null || redirectedWorldParent == null)
        {
            Debug.LogError("RotationGainController: Missing references!");
            enabled = false;
            return;
        }

        lastHeadYaw = head.eulerAngles.y;
    }

    void Update()
    {

        if (!isRedirecting) return;

        float currentYaw = head.eulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(lastHeadYaw, currentYaw);
        lastHeadYaw = currentYaw;

        // Use absolute value so both head directions trigger clockwise VE rotation
        float absDelta = Mathf.Abs(deltaYaw);

        if (absDelta > 0.01f)
        {
            // Smooth the VE rotation per frame
            float veRotation = absDelta * rotationGain;

            // Optional: cap maximum rotation per frame to avoid jumps
            veRotation = Mathf.Min(veRotation, 2f); // max 2 degrees per frame

            // Apply rotation around head
            redirectedWorldParent.RotateAround(head.position, Vector3.up, veRotation);

            accumulatedRotation += veRotation;

            // Stop once VE rotated 90 degrees
            if (accumulatedRotation >= targetRotation)
            {
                isRedirecting = false;
                hasRotatedOnce = true;
                // distractionController?.DeactivateDistraction();
                Debug.Log("✅ VE rotation complete: 90° achieved. Single-use flag set.");
            }
        }
    }

    public void StartRedirection()
    {

        if (hasRotatedOnce) return;
        
        accumulatedRotation = 0f;
        lastHeadYaw = head.eulerAngles.y;
        isRedirecting = true;

        //distractionController?.ActivateDistraction();
        Debug.Log("➡ Rotation gain started (single-use).");
    }

}