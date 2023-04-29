using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetRotation : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float offsetRotation;

    private void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + offsetRotation, 0);
    }
}
