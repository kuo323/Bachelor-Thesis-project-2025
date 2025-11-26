
using System.Collections;

using UnityEngine;

public class RotationGainController : MonoBehaviour
{

    // --- REQUIRED REFERENCES ---
    [Header("Core RDW References")]
    // NOTE: vrCameraRig is kept for setup, but the script pivots around 'head'
    public RoomBounderiesChecker boundaryChecker; // Reference to the checker for single-use logic
    public Transform vrCameraRig;
    public Transform head;                   // The accurate pivot point (Head/Center Eyes) and rotation input
    public Transform redirectedWorldParent;  // The container for all virtual scenery

    // --- DISTRACTION & BOUNDARY SETTINGS ---
    [Header("Distraction & Boundary")]
    public DistractionManager distractionController;
    public float resetBoundaryThreshold = 1.8f;      // Safety distance (e.g., 1.8m)
    public float safetyTimeout = 5.0f;               // Max time to wait for user to turn during reset

    // --- CONTINUOUS GAIN SETTINGS ---
    [Header("Continuous Gain Settings")]
    public float maxGainMultiplier = 1.15f; // Max gain for CCW drift (steers right)
    public float minGainMultiplier = 0.85f; // Min gain for CW drift (steers left)
    public float playAreaWidth = 3.0f;      // Physical play area width (e.g., 3.0m wide)

    // --- PRIVATE STATE VARIABLES ---
    private float lastHeadYaw;
    private bool isResetting = false;
    public bool isRedirectionActive = true;

    // =========================================================================================
    // STARTUP & INITIALIZATION
    // =========================================================================================

    void Start()
    {
        if (head == null || redirectedWorldParent == null || distractionController == null || boundaryChecker == null)
        {
           // Debug.LogError("❌ Missing essential RDW references! Check Head, World Parent, Distraction Manager, and Boundary Checker.");
            enabled = false;
            return;
        }

        // CRITICAL FIX: Align world's pivot to the head's starting position for perfect centering
        redirectedWorldParent.position -= head.position;

        lastHeadYaw = head.eulerAngles.y;
        Debug.Log("✅ RDW Controller initialized. Using Head position as rotation pivot.");
    }

    // =========================================================================================
    // MAIN UPDATE LOOP (Continuous Gain Runs Here)
    // =========================================================================================

    void Update()
    {
        // Stop continuous rotation if a reset is in progress or if redirection is manually deactivated
        if (isResetting || !isRedirectionActive) return;

        ApplyContinuousRotationGain();
    }

    // =========================================================================================
    // CONTINUOUS ROTATION GAIN LOGIC (Subtle Steering)
    // =========================================================================================

    void ApplyContinuousRotationGain()
    {
        float currentHeadYaw = head.eulerAngles.y;
        float deltaRealYaw = Mathf.DeltaAngle(lastHeadYaw, currentHeadYaw);
        lastHeadYaw = currentHeadYaw;

        if (Mathf.Abs(deltaRealYaw) < 0.01f) return;

        // 1. Calculate the dynamic gain factor (alpha) based on head's physical position
        float gainFactor = CalculateDynamicGain(head.position);

        // 2. Calculate the Correction Angle (Delta Virtual - Delta Real)
        float rotationCorrection = deltaRealYaw * (gainFactor - 1f);

        // 3. Apply the Correction to the World Parent around the user's head (Pivot Fix)
        redirectedWorldParent.RotateAround(
            head.position, // The user's accurate head position (pivot point)
            Vector3.up,
            -rotationCorrection
        );
    }

    // --- PATH ALIGNMENT LOGIC ---
    float CalculateDynamicGain(Vector3 physicalPosition)
    {
        float halfWidth = playAreaWidth / 2f;
        float lateralDistance = physicalPosition.x;
        float normalizedX = Mathf.Clamp(lateralDistance / halfWidth, -1f, 1f);

        // Linearly ramp up gain from 1.0 at center to max/min at the edge
        if (normalizedX < 0)
        {
            return Mathf.Lerp(1.0f, maxGainMultiplier, Mathf.Abs(normalizedX));
        }
        else
        {
            return Mathf.Lerp(1.0f, minGainMultiplier, normalizedX);
        }
    }

    // =========================================================================================
    // 180 DEGREE RESET LOGIC (Turning-Based Reset)
    // =========================================================================================

    public void TriggerBoundaryReset()
    {
        if (!isResetting)
        {
            StartCoroutine(BoundaryResetCoroutine());
        }
    }

    private IEnumerator BoundaryResetCoroutine()
    {
        isResetting = true;

        // Use gain of 2.0 (alpha=2.0) to achieve 90 deg virtual turn with 45 deg physical turn
        const float resetGainFactor = 2.0f;

        // Target 90 degrees of virtual environment rotation (as observed in the project)
        float remainingAngleToTurn = 90f;

        float lastYaw = head.eulerAngles.y;
        float startTime = Time.time;

        // 1. Activate the distraction (Visual Mask)
     //   distractionController.ActivateDistraction();
        yield return new WaitForSeconds(0.05f); // Wait a moment for the mask to appear

        // 2. Loop until the target rotation is met OR safety timeout
        while (remainingAngleToTurn > 0.1f && Time.time < startTime + safetyTimeout)
        {
            float currentYaw = head.eulerAngles.y;
            float deltaRealYaw = Mathf.DeltaAngle(lastYaw, currentYaw);
            lastYaw = currentYaw;

            // Only apply correction if the user is actively turning their body/head
            if (Mathf.Abs(deltaRealYaw) > 0.05f)
            {
                // Rotation Correction = deltaRealYaw * (alpha - 1.0f)
                float rotationCorrection = deltaRealYaw * (resetGainFactor - 1.0f);

                float angleToApply = Mathf.Min(Mathf.Abs(rotationCorrection), remainingAngleToTurn);

                // Rotate world opposite to user's turn direction
                float finalCorrection = deltaRealYaw > 0 ? -angleToApply : angleToApply;

                redirectedWorldParent.RotateAround(
                    head.position, // PIVOT: The centered head position
                    Vector3.up,
                    finalCorrection
                );

                remainingAngleToTurn -= angleToApply;
            }

            yield return null; // Wait for the next frame
        }

        // 3. Deactivate the distraction and clean up
     //   distractionController.DeactivateDistraction();

        isResetting = false;
        Debug.Log($"Reset Complete. Final VE rotation applied. Remaining angle: {remainingAngleToTurn} degrees.");

        // NOTE: The ResetBoundaryTrigger() call is REMOVED to meet the single-use requirement.
        // The resetTriggered flag in RoomBounderiesChecker will remain TRUE.

        // Optional: Trigger the path opening visual
        // distractionController.OpenNewPath(); 
    }

}