using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera OnTopCamera;
    [SerializeField] private Vector3 offsetCamPosition;
    [SerializeField] private float zoomAmount = 50.0f;
    [SerializeField] private float zoomDuration = 0.15f;

    Vector3 originalCamPosition;
    Vector3 originalOffsetCamPosition;
    float originalCamFov;

    [Header("Hand Settings")]
    [SerializeField] private Transform mainHandTransform;
    [SerializeField] private Vector3 offsetHandRotation;

    Vector3 originalHandRotation;
    Vector3 originalOffsetHandRotation;

    float zoomTimeElapsed;
    bool aimFinished = true;

    public bool aiming { get; set; }

    [Header("Fire Settings")]
    [SerializeField] private float reloadTime = 2.0f;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private int bulletPerRound = 30; //Cargador
    [SerializeField] private int totalBullet = 180;
    [SerializeField] private LayerMask rayLayerMask;
    [SerializeField] private GameObject impactParticle;
    [SerializeField] private GameObject impactBloodParticle;

    int bulletInRound;
    bool firing;
    public bool Reloading { get; set; }
    Coroutine ShootingRoutine = null;

    PhotonView pView;


    void Start()
    {
        pView = GetComponentInParent<PhotonView>();
        StartCoroutine(InitializeValuesLater());

        bulletInRound = bulletPerRound;
    }

    void Update()
    {

    }

    private void LateUpdate()
    {
        if (!pView.IsMine)
            return;

        if (Input.GetButtonDown("Fire2") && aimFinished)
        {
            originalOffsetCamPosition = offsetCamPosition;
            offsetCamPosition += originalCamPosition;

            originalOffsetHandRotation = offsetHandRotation;
            offsetHandRotation += originalHandRotation;

            zoomTimeElapsed = 0.0f;
            aiming = true;
            aimFinished = false;

            GameController.Instance.UIControllerInstance.SwitchCrossHair(false);
        }
        if (Input.GetButtonUp("Fire2") && aiming)
        {
            aiming = false;
            zoomTimeElapsed = 0.0f;
            Invoke(nameof(ZoomFinished), zoomDuration);

            GameController.Instance.UIControllerInstance.SwitchCrossHair(true);
        }

        if (aiming && !aimFinished)
        {
            LerpZoomIn();
        }
        else if (!aimFinished)
        {
            LerpZoomOut();
        }
    }

    void InitializeValues()
    {
        originalCamPosition = mainCamera.transform.localPosition;
        originalHandRotation = mainHandTransform.localEulerAngles;
        originalCamFov = mainCamera.fieldOfView;

        GetComponent<LeftHandAdjuster>().enabled = true;
    }

    IEnumerator InitializeValuesLater()
    {
        yield return null;
        InitializeValues();
    }

    void LerpZoomIn()
    {
        if (zoomTimeElapsed < zoomDuration)
        {
            mainCamera.transform.localPosition = Vector3.Lerp(originalCamPosition, offsetCamPosition, zoomTimeElapsed / zoomDuration);
            mainHandTransform.transform.localEulerAngles = Vector3.Lerp(originalHandRotation, offsetHandRotation, zoomTimeElapsed / zoomDuration);
            mainCamera.fieldOfView = Mathf.Lerp(originalCamFov, zoomAmount, zoomTimeElapsed / zoomDuration);
            OnTopCamera.fieldOfView = Mathf.Lerp(originalCamFov, zoomAmount, zoomTimeElapsed / zoomDuration);
            zoomTimeElapsed += Time.deltaTime;
        }
        else
        {
            mainCamera.transform.localPosition = offsetCamPosition;
            mainHandTransform.localEulerAngles = offsetHandRotation;
        }
    }

    void LerpZoomOut()
    {
        if (zoomTimeElapsed < zoomDuration)
        {
            mainCamera.transform.localPosition = Vector3.Lerp(offsetCamPosition, originalCamPosition, zoomTimeElapsed / zoomDuration);
            mainHandTransform.transform.localEulerAngles = Vector3.Lerp(offsetHandRotation, originalHandRotation, zoomTimeElapsed / zoomDuration);
            mainCamera.fieldOfView = Mathf.Lerp(zoomAmount, originalCamFov, zoomTimeElapsed / zoomDuration);
            OnTopCamera.fieldOfView = Mathf.Lerp(zoomAmount, originalCamFov, zoomTimeElapsed / zoomDuration);
            zoomTimeElapsed += Time.deltaTime;
        }
        else
        {
            mainCamera.transform.localPosition = originalCamPosition;
            mainHandTransform.localEulerAngles = originalHandRotation;
        }
    }

    void ZoomFinished()
    {
        offsetCamPosition = originalOffsetCamPosition;
        offsetHandRotation = originalOffsetHandRotation;
        aimFinished = true;
    }

    void InstantiateParticles(RaycastHit raycastHit, GameObject particle, bool connected)
    {
        GameObject tempParticle;
        
        if (connected)
        {
            tempParticle = PhotonNetwork.Instantiate(particle.name, raycastHit.point, Quaternion.LookRotation(raycastHit.normal));
        }
        else
        {
            tempParticle = Instantiate(particle, raycastHit.point, Quaternion.LookRotation(raycastHit.normal));
        }
    }
}
