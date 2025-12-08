
using UnityEngine;

public class DoorOpenController : MonoBehaviour
{

    public RotationGainController rotationGainController;
    
    
    [Header("Assign Door & Trigger")]
    public Transform door;            // <-- assign the door here


    [Header("Door Movement Settings")]
    public float targetX = -1.7f;   // Final X position
    public float speed = 1.5f;      // Move speed


    private bool insideTrigger = false; // Tracks if player is inside trigger
    private bool doorOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only react when the correct collider touches AND the tag matches
        if (other.CompareTag("PlayerHand"))
        {
            insideTrigger = true;
            Debug.Log("Hand entered trigger — waiting for trigger press...");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            insideTrigger = false;
        }
    }



    private void Update()
    {
      
        bool rightTrigger = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

        // Open only when inside trigger + pressing trigger
        if (insideTrigger && rightTrigger && !doorOpened && rotationGainController.enableDoor)
        {
            doorOpened = true;
            Debug.Log("Trigger pressed — Door Opening...");
        }

        // Move door
        if (doorOpened && door != null)
        {
            Vector3 pos = door.localPosition;
            pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * speed);
            door.localPosition = pos;
        }
    }

}
