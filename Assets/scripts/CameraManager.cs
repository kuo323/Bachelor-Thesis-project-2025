using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
  
    public static CameraManager Instance { get; private set; }

    public Transform rig;
    public Transform head;
    public Vector3 roomCenter = Vector3.zero;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy the whole GameObject if another instance exists
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: persist across scenes
        Debug.Log("CameraManager Instance assigned!");

        // Optional safety checks
        if (rig == null) Debug.LogWarning("CameraManager: Rig not assigned!");
        if (head == null) Debug.LogWarning("CameraManager: Head not assigned!");
    }

}
