using UnityEngine;

public class RoomBounderiesChecker : MonoBehaviour
{


    private Transform head;
  //  public Vector3 roomCenter = Vector3.zero;

    private Vector3 roomCenter;

    [Header("Room Settings")]
   

    public Vector2 roomSize = new Vector2(4f, 4f); // X and Z size
    public float triggerDistanceFromBoundary = 1f; // distance inside room edge



    private bool wasOutside = false; // Track last frame’s state

    public bool activeDistraction = false;


    


    [Header("Facing Center Settings")]
    public float angleThreshold = 90f;


   

    [SerializeField] private DistractionManager distractionManager;




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
       
        /////checking this 2 conditions by running their calculations in their own functions below /////

        bool facingCenter = IsFacingCenter();
        bool isOutside = IsOutside();




        if (isOutside && !activeDistraction)
        {

            activeDistraction = true;

            distractionManager.SpawnCluster();

            

        }

    


        if (facingCenter && isOutside)
        {

        //    Debug.LogWarning("🚨 Player is looking at the center and it is outside");

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

      

       


    }

    public bool IsOutside()
    {
        if (head == null) return false;

        float halfX = roomSize.x / 2f;
        float halfZ = roomSize.y / 2f;

        // Trigger boundary is 1 meter inside room edge
        float triggerX = halfX - triggerDistanceFromBoundary;
        float triggerZ = halfZ - triggerDistanceFromBoundary;

        Vector3 pos = head.position;

        return Mathf.Abs(pos.x - roomCenter.x) > triggerX ||
               Mathf.Abs(pos.z - roomCenter.z) > triggerZ;
    
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
