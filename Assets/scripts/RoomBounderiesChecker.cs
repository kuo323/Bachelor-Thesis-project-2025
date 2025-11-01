using UnityEngine;

public class RoomBounderiesChecker : MonoBehaviour
{
    
    
    [Header("Player Rig")]
    public Transform rig;

    
    
    [Header("Room Settings")]
    public float boundarySize = 4f;
    public Vector3 roomCenter = new Vector3(0f, 0f, 0f);



    void Start()
    {
        
        rig.transform.position = new Vector3(0, 0, 0);
        Debug.Log("reset position");
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOutside())
        {

            Debug.LogWarning("🚨 Player is outside the boundary!");

        }



    }

    public bool IsOutside()
    {

        Vector3 pos = rig.position;
        float half = boundarySize / 2f;
        return Mathf.Abs(pos.x - roomCenter.x) > half || Mathf.Abs(pos.z - roomCenter.z) > half;

    }



}
