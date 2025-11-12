using UnityEngine;

public class RoomBounderiesChecker : MonoBehaviour
{


    private Transform head;
  //  public Vector3 roomCenter = Vector3.zero;

    private Vector3 roomCenter;

    [Header("Room Settings")]
    public float boundarySize = 4f;
   

    private bool wasOutside = false; // Track last frame’s state

    public bool gainIsApplied = false;

    [Header("Facing Center Settings")]
    public float angleThreshold = 90f;


    

    void Start()
    {

        roomCenter = CameraManager.Instance.roomCenter;
        head = CameraManager.Instance.head;
       

        if (head == null)
        {
            Debug.LogError("Rig not assigned!");
            return;
        }

        head.transform.position = roomCenter;
        Debug.Log("✅ Player position reset to center.");
    }

    void Update()
    {
       
        bool facingCenter = IsFacingCenter();
        
        
        bool isOutside = IsOutside();


      


        if (facingCenter && isOutside)
        {

            Debug.LogWarning("🚨 Player is looking at the center and it is outside");

        }


        // Only log when the state CHANGES
        if (isOutside && !wasOutside)
        {
            Debug.LogWarning("🚨 Player is OUTSIDE the boundary!");

 
            
        }
        else if (!isOutside && wasOutside)
        {
            Debug.Log("✅ Player re-entered the boundary.");
        }

        wasOutside = isOutside;

        gainIsApplied = isOutside && !facingCenter;

    }

    public bool IsOutside()
    {
        if (head == null) return false;

        Vector3 pos = head.position;
        float half = boundarySize / 2f;

        return Mathf.Abs(pos.x - roomCenter.x) > half || Mathf.Abs(pos.z - roomCenter.z) > half;
    }

    public bool IsFacingCenter()
    {

        //// This returns true if the angle is bigger than your threshold ///

        Vector3 toCenter = (roomCenter - head.position).normalized;
        Vector3 forward = new Vector3(head.forward.x, 0, head.forward.z).normalized;

        float angle = Vector3.Angle(forward, toCenter);
        return angle < angleThreshold;
    }
}
