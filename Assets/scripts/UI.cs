using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{


    public RotationGainTrigger rotationGainTrigger;
    public RotationGainController rotationGainController;


    public TMP_Text messageText;
    private bool textChanged = false;
    private bool textChanged2 = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rotationGainTrigger.clusterSpawnedOnce && !textChanged)
        {

            messageText.text = "Press Trigger on the right controller to catch the fireflies";
            messageText.color = Color.yellow;
            textChanged = true;
        }

        if(rotationGainController.hasRotatedOnce && !textChanged2)
        {

            messageText.text = "Great job! You’ve caught enough fireflies! You can move forward now.";
            messageText.color = Color.green;
            textChanged2 = true;
        }



    }
}
