using UnityEngine;



[RequireComponent(typeof(LineRenderer))]

public class BoundaryVisualizer : MonoBehaviour
{
    public float boundarySize = 4f;          // 4x4 room
    public Vector3 roomCenter = Vector3.zero; // Center of room (Y ignored)
    private LineRenderer line;
    private float HeightFromFloor = 0.1f;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 5; // 4 corners + closing point

        float half = boundarySize / 2f;

        // Define corners on the XZ plane (Y = roomCenter.y)
        Vector3[] corners = new Vector3[5];
        corners[0] = new Vector3(roomCenter.x - half, roomCenter.y+HeightFromFloor, roomCenter.z - half);
        corners[1] = new Vector3(roomCenter.x + half, roomCenter.y+HeightFromFloor, roomCenter.z - half);
        corners[2] = new Vector3(roomCenter.x + half, roomCenter.y+HeightFromFloor, roomCenter.z + half);
        corners[3] = new Vector3(roomCenter.x - half, roomCenter.y+HeightFromFloor, roomCenter.z + half);
        corners[4] = corners[0]; // close the loop

        line.SetPositions(corners);
    }
}
