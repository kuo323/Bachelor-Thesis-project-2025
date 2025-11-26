using UnityEngine;

public class RoomBounderiesChecker : MonoBehaviour
{
    // --- REQUIRED REFERENCES ---
    [Header("Core References")]
    public RotationGainController rotationGainController;
    public Transform head; // The user's current physical position

    // --- ROOM AND CHECK SETTINGS ---
    [Header("Tunnel/Boundary Settings")]
    // The Z-coordinate where the reset should be triggered (the end of the tunnel)
    public float tunnelEndTriggerZ = 3.0f;

    // The width of the play area (used for continuous gain, kept here for context)
    public float playAreaWidth = 3.0f;

    // This flag is crucial to prevent multiple reset coroutines from stacking
    private bool resetTriggered = false;

    // =========================================================================

    void Start()
    {
        if (head == null || rotationGainController == null)
        {
            Debug.LogError("❌ Missing Head or RotationGainController reference! Script disabled.");
            enabled = false;
            return;
        }

        // Set the room center to the origin (0, 0, 0) as is standard for RDW setup.
        // We will only check the Z-axis for the tunnel end trigger.
        // roomCenter = Vector3.zero; // Note: We don't use this variable in the new Z-axis check

        // Ensure the rotation controller knows the play area width
        rotationGainController.playAreaWidth = playAreaWidth;

        Debug.Log("✅ RoomBounderiesChecker initialized. Monitoring Z-axis for tunnel end.");
    }

    void Update()
    {
        // 1. Activate/Deactivate Continuous Gain based on X-axis (Lateral Position)
        bool isLateralBoundaryHit = IsLaterallyOutside();
        rotationGainController.isRedirectionActive = isLateralBoundaryHit;


        // 2. Check for the Hard Boundary Reset (Tunnel End)
        bool isTunnelEndHit = IsTunnelEndHit();

        if (isTunnelEndHit)
        {
            // If user passed the end of the tunnel AND we are not already resetting:
            if (!resetTriggered)
            {
                // Trigger the Turning-Based Reset
                rotationGainController.TriggerBoundaryReset();
                resetTriggered = true; // This stays TRUE forever, preventing re-triggering

                Debug.LogWarning("🚨 Hard Boundary Triggered! Initiating Turning-Based Reset (Single Use).");
            }
        }

        // DELETE THE 'ELSE' BLOCK that would reset the flag based on position.
    }

    // =========================================================================
    // BOUNDARY CHECK FUNCTIONS
    // =========================================================================

    // Checks if the user has reached the end of the tunnel (Z-axis boundary)
    public bool IsTunnelEndHit()
    {
        if (head == null) return false;

        // Trigger when the user's Z-position exceeds the set tunnel end coordinate.
        return head.position.z > tunnelEndTriggerZ;
    }

    // Checks if the user is outside the safety margin on the sides (X-axis)
    public bool IsLaterallyOutside()
    {
        if (head == null) return false;

        // Use the lateral threshold from the rotation controller (e.g., half the play area width)
        float halfWidth = playAreaWidth / 2f;

        // This check determines when the subtle continuous gain should be active
        return Mathf.Abs(head.position.x) > (halfWidth * 0.5f); // Example: start steering when halfway to the edge
    }

    // Public function called by the RotationGainController after the reset coroutine finishes
  
}
