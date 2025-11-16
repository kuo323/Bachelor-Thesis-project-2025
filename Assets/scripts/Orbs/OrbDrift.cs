using Unity.VisualScripting;
using UnityEngine;

public class OrbDrift : MonoBehaviour
{
    private Vector3 startPos;
    private float driftSpeed;
    private float driftAmount;
    private Vector3 randomDir;


    private bool isHit = false;
    private float hitTimer = 0f;
    private float hitDuration = 2f;



    private float normalDriftAmount;   // <- store original value
    private float panicDriftAmount = 2.5f; // <- panic mode

    void Start()
    {
        startPos = transform.localPosition;
        driftSpeed = Random.Range(0.5f, 1.5f);
        driftAmount = Random.Range(0.5f, 0.5f);  //// Sets how far each object can drift from its starting position.


        randomDir = Random.onUnitSphere;   //// Generates a random unit vector in 3D space (direction) for the drift.
        randomDir.y = Mathf.Abs(randomDir.y); // Makes the Y component of the drift positive, so the object tends to move upward instead of downward.


        normalDriftAmount = driftAmount;  //// store the reset value


}

    void Update()
    {

        /////If isHit is true, then targetAmount = panicDriftAmount (e.g., 2.5f). If isHit is false, then targetAmount = normalDriftAmount(e.g., 0.5f).////
        float targetAmount = isHit ? panicDriftAmount : normalDriftAmount;

        driftAmount = Mathf.Lerp(driftAmount, targetAmount, Time.deltaTime * 4f); // smooth transition
        

        // Floating movement
        transform.localPosition = startPos + randomDir * Mathf.Sin(Time.time * driftSpeed) * driftAmount;



        if (isHit)
        {

            //// starts counting up //// 
            hitTimer += Time.deltaTime;

            if (hitTimer >= hitDuration)
            {

                isHit = false;
                hitTimer = 0f;
                // NO instant reset of driftAmount here!

            }


        }





    }


    public void Oncehit()
    {

        isHit = true;
        hitTimer = 0f;

        
    }


}
