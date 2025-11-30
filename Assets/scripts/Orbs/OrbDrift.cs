
using UnityEngine;

public class OrbDrift : MonoBehaviour
{
    private Vector3 startPos;

    // drift parameters
    private float driftAmount;
    private float normalDriftAmount;
    private float panicDriftAmount = 9.0f;  //// how far they spread out

    // random noise offsets so each orb moves differently
    private float noiseX;
    private float noiseY;
    private float noiseZ;

    private float floatSpeed;
    private float wanderSpeed;

    private bool isHit = false;
    private float hitTimer = 0f;
    private float hitDuration = 10f;

    void Start()
    {
        startPos = transform.localPosition;

        floatSpeed = Random.Range(3f, 3f);       // vertical floating
        wanderSpeed = Random.Range(3f, 3f);  // horizontal wandering

        driftAmount = Random.Range(7.0f, 4.5f);
        normalDriftAmount = driftAmount;

        // give this orb its own perlin noise seed values
        noiseX = Random.Range(800f, 999f);
        noiseY = Random.Range(700f, 999f);
        noiseZ = Random.Range(100f, 999f);
    }

    void Update()
    {
        // increase drift when hit (panic mode)
        float targetAmount = isHit ? panicDriftAmount : normalDriftAmount;
        driftAmount = Mathf.Lerp(driftAmount, targetAmount, Time.deltaTime * 4f);

        // PERLIN-NOISE FIRELFY MOVEMENT ----------------------------

        float offsetX =
            (Mathf.PerlinNoise(Time.time * wanderSpeed, noiseX) - 0.8f)
            * driftAmount;

        float offsetZ =
            (Mathf.PerlinNoise(Time.time * wanderSpeed, noiseZ) - 0.8f)
            * driftAmount;

        float offsetY =
            Mathf.Sin(Time.time * floatSpeed + noiseY)
            * (driftAmount * 0.5f);   // vertical floating is smaller

        // apply combined floating + wandering
        transform.localPosition = startPos + new Vector3(offsetX, offsetY, offsetZ);

        //------------------------------------------------------------

        // handle hit timer
        if (isHit)
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= hitDuration)
            {
                isHit = false;
                hitTimer = 0f;
            }
        }
    }

    public void Oncehit()
    {
        isHit = true;
        hitTimer = 0f;
    }


}
