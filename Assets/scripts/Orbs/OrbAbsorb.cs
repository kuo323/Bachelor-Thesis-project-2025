using UnityEngine;

public class OrbAbsorb : MonoBehaviour
{
    private bool isTouched = false;

    private OrbDrift orbDrift;




    private void Start()
    {

        // grab the OrbDrift script dynamically when it is not hooked on any object 
        orbDrift = GetComponent<OrbDrift>();

    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            isTouched = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            isTouched = false;
        }
    }

    private void Update()
    {
        bool rightTrigger = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

        if (isTouched && rightTrigger)
        {


            // Get parent object of the objects tthat just get hit 
            Transform cluster = transform.parent;
           //// Loop through all orbs in the cluster with foreach
            foreach (Transform orb in cluster)
            {
                if (orb == transform) continue; // skip the hit orb

                OrbDrift drift = orb.GetComponent<OrbDrift>();
                if (drift != null)
                    drift.Oncehit();
            }

            // Absorb/destroy the hit orb
            Absorb();
        }
    }

    private void Absorb()
    {
        // Optional: play particle, sound, animation here
        Destroy(gameObject);
    }
}
