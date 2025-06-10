using System;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera targetCamera;

    [SerializeField] private bool invertFacing;
    
    private void Start()
    {
        targetCamera = GameManager.Instance.mainCamera;
    }

    private void Update()
    {
        // Ensure the 'up' vector aligns with the world Y-axis
        transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.back, targetCamera.transform.rotation * Vector3.up);

    }
}
