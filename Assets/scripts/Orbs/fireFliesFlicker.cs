using UnityEngine;
using System.Collections;

public class fireFliesFlicker : MonoBehaviour
{

    private Material emissionMaterial;
    private float currentIntensity = 0f;
    private float targetIntensity = 0f;

    [Header("Flicker Settings")]
    public float intensityLerpSpeed = 2f;
    public float minIntensity = 0f;
    public float maxIntensity = 1.5f;
    public float minOnTime = 0.5f;
    public float maxOnTime = 2f;
    public float minOffTime = 0.2f;
    public float maxOffTime = 1.2f;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();

        // If no renderer on this object, get it from children
        if (rend == null)
            rend = GetComponentInChildren<Renderer>();

        if (rend == null)
        {
            Debug.LogError("fireFliesFlicker: No Renderer found on this object or its children!", this);
            enabled = false;
            return;
        }

        // Make per-orb material instance
        emissionMaterial = new Material(rend.material);
        rend.material = emissionMaterial;

        StartCoroutine(FlickerRoutine(Random.Range(0f, 1f)));
    }

    void Update()
    {
        // Smoothly interpolate intensity
        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * intensityLerpSpeed);
        emissionMaterial.SetColor("_EmissionColor", Color.yellow * currentIntensity);
    }

    private IEnumerator FlickerRoutine(float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // Turn on with random brightness
            targetIntensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(Random.Range(minOnTime, maxOnTime));

            // Turn off
            targetIntensity = 0f;
            yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));
        }
    }
}
