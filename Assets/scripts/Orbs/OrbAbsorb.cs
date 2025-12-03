using UnityEngine;

public class OrbAbsorb : MonoBehaviour
{


    [HideInInspector] public GameObject absorbFXPrefab;   // LATER LINK THIS TO DISTRACTION MANAGER
  



    /// <summary>
    /// grab the rotationGainController reference from DistractionManager 
    /// cause this script is a dynamic script which can not grab the rotationGainController reference itself ditrectly 
    /// </summary>
    [HideInInspector] public RotationGainController rotationGainController; // set from DistractionManager
    [HideInInspector] public DistractionManager distractionManager; // new reference

    private OrbDrift orbDrift;


    private bool isTouched = false;
    private bool isBeingAbsorbed = false;


    public float absorbDuration = 2f;  // how fast the orb shrinks & gets sucked in
    public float absorbSpeed = 5f;        // movement speed toward stick

    private Transform absorberStick;      // reference to the stick that touched the orb


   




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

        

        // --- Absorb logic ---
        if (isTouched && rightTrigger && !isBeingAbsorbed)
        {

            // Trigger rotation burst only when pressing trigger
            rotationGainController.StartRedirection();

            distractionManager?.afterHit();

            Transform cluster = transform.parent;
            foreach (Transform orb in cluster)
            {
                if (orb == transform) continue;

                OrbDrift drift = orb.GetComponent<OrbDrift>();
                if (drift != null)
                    drift.Oncehit();
            }

            StartCoroutine(Absorb());
        }
    }

    private System.Collections.IEnumerator Absorb()
    {
        isBeingAbsorbed = true;

        Vector3 initialScale = transform.localScale;
        float t = 0f;

        while (t < absorbDuration)
        {
            t += Time.deltaTime;

            // shrink
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t / absorbDuration);

            // move toward stick Z direction (forward)
            if (absorberStick != null)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    absorberStick.position + absorberStick.forward * 0.1f,
                    absorbSpeed * Time.deltaTime
                );
            }

            yield return null;
        }


        // Notify manager that this orb is fully absorbed
        distractionManager?.OrbAbsorbed();

        // Spawn and then destry the particle FX of the orbs 
        if (absorbFXPrefab != null)
        {
            GameObject fx = Instantiate(absorbFXPrefab, transform.position, Quaternion.identity);
            Destroy(fx, 2f); // auto cleanup
        }


        //destroy the orbs 
        Destroy(gameObject);
    }



  

}
