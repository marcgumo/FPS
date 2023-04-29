using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float sensitivity = 2.0f;
    [SerializeField] private float minPitch = -60f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] bool inverseMouse = false;

    [Header("Bone Settings")]
    [SerializeField] Transform boneParent;

    bool updateCamera = true;

    public float pitch { get; set; }
    //same above with yaw
    public float yaw { get; set; }
    //relative yaw
    public float relativeYaw { get; set; }

    PhotonView pView;

    void Start()
    {
        pView = GetComponent<PhotonView>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(!pView.IsMine)
        {
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<Camera>().GetComponent<AudioListener>().enabled = false;
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            updateCamera = !updateCamera;
            Cursor.lockState = updateCamera ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !updateCamera;
        }
#endif

        if(!pView.IsMine)
            return;
        
        if (!updateCamera)
            return;
        UpdateCamera();
    }

    private void LateUpdate()
    {
        transform.position = boneParent.position;
    }

    private void UpdateCamera()
    {
        Vector2 mouseInput =  new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if(inverseMouse)
        {
            mouseInput.y *= -1;
        }

        relativeYaw = mouseInput.x * sensitivity;
        pitch -= mouseInput.y * sensitivity;
        yaw += mouseInput.x * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
