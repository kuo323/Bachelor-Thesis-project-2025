using UnityEngine;

public class DoorSensorFlicker : MonoBehaviour
{
    [Header("Material Settings")]
    public Renderer targetRenderer;           // Assign the renderer here
    public int emissionMaterialIndex = 0;     // Which material in the array to flicker
    public Color baseEmissionColor = Color.yellow;
    public float minIntensity = 0.1f;
    public float maxIntensity = 1.0f;
    public float flickerSpeed = 5f;

    private Material flickerMat;

    void Start()
    {
        if (targetRenderer != null && targetRenderer.materials.Length > emissionMaterialIndex)
        {
            // Create an instance of the target material so we don't change the shared material
            Material[] mats = targetRenderer.materials;
            flickerMat = mats[emissionMaterialIndex];
            flickerMat.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogError("Emission material index out of range!");
        }
    }

    void Update()
    {
        if (flickerMat == null) return;

        float intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(Time.time * flickerSpeed, 0f));
        flickerMat.SetColor("_EmissionColor", baseEmissionColor * intensity);
    }


}
