using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{


    public RotationGainTrigger rotationGainTrigger;
    public RotationGainController rotationGainController;


    // Replace single TMP text with two UI objects instead
    public GameObject firstMessageUI;
    public GameObject secondMessageUI;
    public GameObject thirdMessageUI;

    private bool firstShown = false;
    private bool secondShown = false;

    
    void Start()
    {
        // Start hidden (optional)
        thirdMessageUI.SetActive(false);
        secondMessageUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Show first UI instruction
        if (rotationGainTrigger.clusterSpawnedOnce && !firstShown)
        {
            firstMessageUI.SetActive(false);
            secondMessageUI.SetActive(true);

            firstShown = true;
        }

        // Show second message & hide first if you want
        if (rotationGainController.hasRotatedOnce && !secondShown)
        {
            secondMessageUI.SetActive(false);      // optional
            thirdMessageUI.SetActive(true);
            secondShown = true;
        }



    }
}
