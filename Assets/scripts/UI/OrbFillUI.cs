using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OrbFillUI : MonoBehaviour
{
    [Header("UI Images (Bottom to Top)")]
    public List<Image> orbImages = new List<Image>();

    [Header("Fill Settings")]
    public float fillPerOrb = 0.3f; // how much each orb increases

    private int currentIndex = 0;   // which image is being filled



public void AbsorbCount()
    {


        if (currentIndex >= orbImages.Count) return; // all full

        Image currentImage = orbImages[currentIndex];
        currentImage.fillAmount += fillPerOrb;

        // Clamp to 1
        if (currentImage.fillAmount >= 1f)
        {
            currentImage.fillAmount = 1f;
            currentIndex++; // move to next image
        }



    }




}
