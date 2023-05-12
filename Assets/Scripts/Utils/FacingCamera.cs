using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (Camera.main == null)
            return;

        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward);
    }
}
