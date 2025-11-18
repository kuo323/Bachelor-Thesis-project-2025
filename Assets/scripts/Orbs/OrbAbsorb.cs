using UnityEngine;

public class OrbAbsorb : MonoBehaviour
{

    /// <summary>
    /// grab the rotationGainController reference from DistractionManager 
    /// cause this script is a dynamic script which can not grab the rotationGainController reference itself ditrectly 
    /// </summary>
    [HideInInspector] public RotationGainController rotationGainController; // set from DistractionManager
    [HideInInspector] public DistractionManager distractionManager; // new reference

    private bool isTouched = false;

    private OrbDrift orbDrift;

    




    private void Start()
    {

        // grab the OrbDrift script dynamically when it is not hooked on any object 
        orbDrift = GetComponent<OrbDrift>();

        if (rotationGainController == null)
            Debug.LogError("RotationGainController reference not set on orb!");

        if (distractionManager == null)
            Debug.LogError("DistractionManager reference not set on orb!");
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


            // Apply rotation gain first

           rotationGainController?.ApplyRotationGain();



            // Notify the cluster to speed up
            distractionManager?.afterHit();


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



            
             Absorb();

            


        }
    }

    private void Absorb()
    {
        // Optional: play particle, sound, animation here
        Destroy(gameObject);
    }



  




}
