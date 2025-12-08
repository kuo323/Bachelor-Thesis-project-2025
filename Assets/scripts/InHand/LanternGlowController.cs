using UnityEngine;

public class LanternGlowController : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource absorbSound; // assign an AudioSource in the Inspector


    [Header("Light Settings")]
    public Light candleLight;              // Point Light inside the lantern
    public float intensityMultiplier = 2f; // Multiplies emission value for the light

    [Header("Emission Settings")]
    
    public float minFlicker = 0.1f;        // Min emission when idle
    public float maxFlicker = 0.3f;        // Max emission when idle
    public float flickerSpeed = 10f;       // Speed of idle flicker
    public float absorbIncrement = 0.1f;   // Emission increase per absorbed orb
    public float maxEmission = 1.0f;       // Max emission allowed
    public float decaySpeed = 1f;          // Speed to fade back to idle

    
    private float currentEmission;
    private bool flickering = true;

    void Start()
    {
       
       

        // Start at base emission
        currentEmission = minFlicker;
        SetEmission(currentEmission);
    }

    void Update()
    {
        if (flickering)
        {
            float flicker = Mathf.Lerp(minFlicker, maxFlicker, Mathf.PerlinNoise(Time.time * flickerSpeed, 0f));
            currentEmission = flicker;
            SetEmission(currentEmission);
        }

    }

    /// <summary>
    /// Call this function when a firefly orb is absorbed
    /// </summary>
    public void AbsorbBoost()
    {

        flickering = false;
        currentEmission += absorbIncrement;
        currentEmission = Mathf.Clamp(currentEmission, 0f, maxEmission);

        SetEmission(currentEmission); // apply the change immediately

        // Play sound
        if (absorbSound != null)
            absorbSound.PlayOneShot(absorbSound.clip);
    }


    /// <summary>
    /// Set both emission and light intensity
    /// </summary>
    private void SetEmission(float value)
    {
       

        if (candleLight != null)
        {
            candleLight.intensity = value * intensityMultiplier;
        }
    }
}
