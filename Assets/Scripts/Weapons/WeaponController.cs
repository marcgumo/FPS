using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private int damage = 10;
    [SerializeField] private int bulletsPerRound = 30; //Cargador
    [SerializeField] private int totalBullets = 180;
    [SerializeField] private LayerMask rayLayerMask;
    [SerializeField] private GameObject impactParticle;
    [SerializeField] private GameObject impactBloodParticle;

    [Header("Recoil Settings")]
    [SerializeField, Range(0.0f, 1.0f)] private float verticalRecoilAmount = 0.2f;
    [SerializeField, Range(0.0f, 1.0f)] private float horizontalRecoilAmount = 0.1f;
    [SerializeField, Range(5.0f, 15.0f)] private float maxVerticalRecoil = 10.0f;
    [SerializeField] private float horizontalRecoilAmountTop = 2.0f;

    float verticalRecoilTotal;
    float verticalRecoil;
    float horizontalRecoil;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip reloadSound;
    AudioSource audioSource;

    int bulletsInRound;
    bool firing;
    public bool Reloading { get; set; }
    Coroutine ShootingRoutine = null;

    PhotonView pView;

    Animator anim;

    void Start()
    {
        pView = GetComponentInParent<PhotonView>();
        anim = GetComponentInParent<Animator>();

        StartCoroutine(InitializeValuesLater());

        bulletsInRound = bulletsPerRound;

        verticalRecoil = verticalRecoilAmount;
        horizontalRecoil = horizontalRecoilAmount;
    }

    void Update()
    {
        if (!pView.IsMine)
            return;

        if (Input.GetButtonDown("Fire1") && !firing && !Reloading && bulletsInRound > 0)
        {
            firing = true;

            SpawnRaycast();
            bulletsInRound--;

            ShootingRoutine = StartCoroutine(ShootBullet());
        }
        else if (Input.GetButtonUp("Fire1") || bulletsInRound == 0)
        {
            firing = false;
            verticalRecoilTotal = 0.0f;
            StopCoroutine(ShootingRoutine);
        }

        if (Input.GetKeyDown(KeyCode.R) && !Reloading && bulletsInRound < bulletsPerRound && totalBullets > 0)
        {
            Reloading = true;
            Invoke(nameof(ReloadFinished), reloadTime);

            firing = false;
            StopCoroutine(ShootingRoutine);

            anim.SetLayerWeight(2, 0.7f);
        }

        UpdateAnimation();

        GameController.Instance.UIControllerInstance.BulletTextUpdate(bulletsInRound, totalBullets);
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

    void SpawnRaycast()
    {
        RaycastHit raycastHit;
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

        if (Physics.Raycast(ray, out raycastHit, 1000.0f, rayLayerMask.value))
        {
            if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                InstantiateParticles(raycastHit, impactParticle, PhotonNetwork.IsConnected);
            }
            else if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                InstantiateParticles(raycastHit, impactBloodParticle, PhotonNetwork.IsConnected);
            }
        }

        AddRecoil();
    }

    void AddRecoil()
    {
        //save the totat vertical recoil
        verticalRecoilTotal += verticalRecoil;

        //if total vertical recoil is greater than the max vertical recoil, dont add any more recoil
        if (verticalRecoilTotal >= maxVerticalRecoil)
        {
            verticalRecoilTotal = maxVerticalRecoil;
            verticalRecoil = horizontalRecoilAmount;

            horizontalRecoil = horizontalRecoilAmountTop;

            print("Maximo recoil");
        }
        else if (!Input.GetButton("Fire2"))
        {
            verticalRecoil = verticalRecoilAmount;
            horizontalRecoil = horizontalRecoilAmount;
        }
        else
        {
            verticalRecoil = verticalRecoilAmount * 0.5f;
            horizontalRecoil = horizontalRecoilAmount * 0.5f;
        }

        //add a random positive vertical recoil to the mainCamera parent
        mainCamera.GetComponentInParent<CameraController>().pitch += Random.Range(
            -verticalRecoil, verticalRecoilTotal >= maxVerticalRecoil ? verticalRecoil : 0.0f);

        //add a random horizontal recoil to the mainCamera
        mainCamera.GetComponentInParent<CameraController>().yaw += Random.Range(-horizontalRecoil, horizontalRecoil);
    }

    IEnumerator ShootBullet()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);

            if (bulletsInRound > 0)
            {
                SpawnRaycast();
                bulletsInRound--;
            }
        }
    }

    void ReloadFinished()
    {
        Reloading = false;

        int bulletsToRemove = bulletsPerRound - bulletsInRound;

        if (totalBullets >= bulletsPerRound)
        {
            bulletsInRound = bulletsPerRound;
            totalBullets -= bulletsToRemove;
        }
        else if (bulletsToRemove <= totalBullets)
        {
            bulletsInRound += bulletsToRemove;
            totalBullets -= bulletsToRemove;
        }
        else
        {
            bulletsInRound += totalBullets;
            totalBullets -= totalBullets;
        }

        anim.SetLayerWeight(2, 0.4f);
    }

    void UpdateAnimation()
    {
        anim.SetBool("shoot", firing);
        anim.SetBool("reload", Reloading);
        anim.SetBool("aim", aiming);
    }
}
