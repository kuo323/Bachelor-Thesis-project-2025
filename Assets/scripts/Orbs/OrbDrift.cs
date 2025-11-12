using UnityEngine;

public class OrbDrift : MonoBehaviour
{
    private Vector3 startPos;
    private float driftSpeed;
    private float driftAmount;
    private Vector3 randomDir;

    void Start()
    {
        startPos = transform.localPosition;
        driftSpeed = Random.Range(0.5f, 1.5f);
        driftAmount = Random.Range(3.0f, 3.0f);  //// Sets how far the object can drift from its starting position.
        randomDir = Random.onUnitSphere;   //// Generates a random unit vector in 3D space (direction) for the drift.
        randomDir.y = Mathf.Abs(randomDir.y); // Makes the Y component of the drift positive, so the object tends to move upward instead of downward.
    }

    void Update()
    {
        transform.localPosition = startPos + randomDir * Mathf.Sin(Time.time * driftSpeed) * driftAmount;
    }
}
