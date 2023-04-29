using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerIK : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private Transform spineToOrientate;

    Vector3 eulerRotation;

    private void LateUpdate()
    {
        eulerRotation = transform.eulerAngles;
        
        spineToOrientate.rotation = Quaternion.Euler(eulerRotation);
    }
}
