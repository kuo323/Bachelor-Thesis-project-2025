
using System.Collections;

using UnityEngine;

public class RotationGainController : MonoBehaviour
{
    public DistractionManager distractionController;
    public GameObject psObject;
    public GameObject arrowUI;

    // ⬇ XR Rig reference (NEW) — assign your XR Rig here in inspector
    public Transform xrRig;

    public float rotationGain = 0.1f;
    private Transform head;

    public float targetRotation = 90f;
    public bool isRedirecting = false;
    private float accumulatedRotation = 0f;

    public bool hasRotatedOnce = false;

    private float gainDuration = 0f;
    public float burstDuration = 1.5f;

    private float lastHeadYaw;


    /// <summary>
    /// Notify the door that it can be opened
    /// </summary>
    public bool enableDoor = false;

    void Start()
    {
        head = CameraManager.Instance.head;

        if (head == null || xrRig == null)
        {
            Debug.LogError("⚠ RotationGainController: Missing XR Rig or Head reference!");
            enabled = false;
            return;
        }

        lastHeadYaw = head.eulerAngles.y;

        arrowUI.SetActive(false);
    }

    void Update()
    {
        if (hasRotatedOnce)
        {
            isRedirecting = false;
            return;
        }

        // Burst time window
        gainDuration -= Time.deltaTime;
        if (gainDuration > 0f) isRedirecting = true;
        else isRedirecting = false;

        if (!isRedirecting) return;

        // Compute user head rotation
        float currentYaw = head.eulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(lastHeadYaw, currentYaw);
        lastHeadYaw = currentYaw;

        if (Mathf.Abs(deltaYaw) < 0.01f) return;

        float veRotation = Mathf.Min(Mathf.Abs(deltaYaw) * rotationGain, 2f);

        // 🏆 NEW — Rotate the XR Rig around user head
        xrRig.RotateAround(head.position, Vector3.up, veRotation);

        accumulatedRotation += veRotation;

        if (accumulatedRotation >= targetRotation)
        {
            hasRotatedOnce = true;
            isRedirecting = false;
            DisableParticleObject();
            EnableArrow();
            EnableDoorOpen();


            Debug.Log("🎉 90° rotation achieved — redirection completed.");
        }
    }

    public void StartRedirection()
    {
        if (hasRotatedOnce) return;

        gainDuration = burstDuration;
        lastHeadYaw = head.eulerAngles.y;
        isRedirecting = true;
    }

    void DisableParticleObject()
    {
        if (psObject != null)
            psObject.SetActive(false);
    }

    void EnableArrow()
    {
        if (arrowUI != null)
            arrowUI.SetActive(true);
    }

    void EnableDoorOpen()
    {
        enableDoor = true;
    }



}

