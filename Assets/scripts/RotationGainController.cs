using UnityEngine;

public class RotationGainController : MonoBehaviour
{

   
    private Transform rig;
    private Transform head;



    [Header("GainSettings")]
    private float gainDuration = 0f;
    public float burstDuration = 0.5f; // how long gain stays active


    [Header("Settings")]
    public float gainMultiplier = 1.5f;  // 1.05 = 5% rotation gain
    public RoomBounderiesChecker boundaryChecker;

    private float lastHeadYaw;

    void Start()
    {
       
        if (CameraManager.Instance == null)
        {
            //make sure the script can get the references from the CameraManager.Instance ///
            Debug.LogError("❌ CameraManager.Instance is null — make sure it's in the scene.");
           // return;
        }

        rig = CameraManager.Instance.rig;
        head = CameraManager.Instance.head;


        if (head == null) Debug.LogError("Head Transform not assigned!");
        if (rig == null) Debug.LogError("Rig Transform not assigned!");

        lastHeadYaw = head.eulerAngles.y;


    }

    void Update()
    {


      


        if (gainDuration > 0f)
        {
            
            
            
            float currentYaw = head.eulerAngles.y;
            float deltaYaw = Mathf.DeltaAngle(lastHeadYaw, currentYaw);

            float virtualRotation = deltaYaw * (gainMultiplier - 1f);
            rig.Rotate(Vector3.up, virtualRotation);

            gainDuration -= Time.deltaTime;


            Debug.Log($"deltaYaw={deltaYaw} virtualRotation={virtualRotation}");



        }




        lastHeadYaw = head.eulerAngles.y;
    }

    public void ApplyRotationGain()
    {


        gainDuration = burstDuration;

        Debug.Log("Rotation gain triggered! burstDuration=" + burstDuration);


    }




   

}