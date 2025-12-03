
using System.Collections;

using UnityEngine;

public class RotationGainController : MonoBehaviour
{
    public DistractionManager distractionController;
    public GameObject psObject; // Assign the ParticleSystem GameObject
    public GameObject arrowUI;

    public Transform redirectedWorldParent;
    public float rotationGain = 0.1f;

    private Transform head;
    private float lastHeadYaw;


    
    public float targetRotation = 90f;     // total VE rotation goal
    public bool isRedirecting = false;
    private float accumulatedRotation = 0f;

    //single-use flag
    public bool hasRotatedOnce = false;



    //new burst gain for collider ///
    private float gainDuration = 0f;
    public float burstDuration = 1.5f; // how long gain stays active

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


        arrowUI.SetActive(false);



    }

    void Update()
    {
        // Stop everything if VE rotation reached 90°
        if (hasRotatedOnce)
        {
            isRedirecting = false;
            return;
        }

        // Count down burst duration
        if (gainDuration > 0f)
        {
            gainDuration -= Time.deltaTime;
            if (!isRedirecting) isRedirecting = true;
        }
        else
        {
            isRedirecting = false;
        }

        if (!isRedirecting) return;

        // Compute rotation delta
        float currentYaw = head.eulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(lastHeadYaw, currentYaw);
        lastHeadYaw = currentYaw;

        float absDelta = Mathf.Abs(deltaYaw);
        if (absDelta < 0.01f) return;

        float veRotation = Mathf.Min(absDelta * rotationGain, 2f);
        redirectedWorldParent.RotateAround(head.position, Vector3.up, veRotation);

        accumulatedRotation += veRotation;

        // Stop permanently at 90°
        if (accumulatedRotation >= targetRotation)
        {
            isRedirecting = false;
            hasRotatedOnce = true;
            DisableParticleObject();
            EnableArrow();
            Debug.Log("✅ VE rotation complete: 90° reached. No more bursts allowed.");
        }
    }

    public void StartRedirection()
    {

        if (hasRotatedOnce) return;

        gainDuration = burstDuration;
        lastHeadYaw = head.eulerAngles.y;
        isRedirecting = true;

        Debug.Log("➡ Rotation gain burst started.");
    }



    void DisableParticleObject()
    {
        if (psObject != null)
        {
            psObject.SetActive(false); // Disables everything on that object
        }



       


    }

    void EnableArrow()
    {

        if (arrowUI != null)
        {

            arrowUI.SetActive(true);
        }


    }

}