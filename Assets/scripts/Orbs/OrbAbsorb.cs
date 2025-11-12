using UnityEngine;

public class OrbAbsorb : MonoBehaviour
{
    public float absorbTime = 1.0f;
    private float touchTimer = 0f;
    private bool absorbed = false;

    public System.Action OnAbsorbed; // Event for parent to handle

    private void OnTriggerStay(Collider other)
    {
        if (absorbed) return;

        // Assuming player's hand or controller has tag "PlayerHand"
        if (other.CompareTag("PlayerHand"))
        {
            touchTimer += Time.deltaTime;

            if (touchTimer >= absorbTime)
            {
                Absorb();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            touchTimer = 0f; // reset timer if player stops touching
        }
    }

    void Absorb()
    {
        absorbed = true;

        // Shrink and fade
        StartCoroutine(ShrinkAndDisappear());

        // Notify parent
        OnAbsorbed?.Invoke();
    }

    System.Collections.IEnumerator ShrinkAndDisappear()
    {
        float t = 0f;
        Vector3 startScale = transform.localScale;
        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        Destroy(gameObject);
    }
}
