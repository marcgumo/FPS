using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float walkSpeedMove = 4f;
    [SerializeField] private float runSpeedMove = 6f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float gravity = -14f;
    bool isRunning;

    [SerializeField] private float aimingSpeedMove = 2.0f;
    bool isAiming = false;

    PhotonView pView;
    float speedMove;
    CharacterController charControl;
    Vector3 moveDir;

    Vector3 verticalVelocity;

    [Header("Ground Detection Settings")]
    [SerializeField] private Transform groundPosition;
    [SerializeField, Range(0f, 1f)] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    bool canDoubleJump;

    [Header("UI Settings")]
    //canvas object
    [SerializeField] private GameObject canvasPlayer;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI usernameText;

    CameraController cameraController;

    Animator anim;
    float turnAmount;
    float mouseXSmooth;

    bool backwards;
    float vInput;

    bool strafe;
    float hInput;
    float horizontalAnimSmooth;

    void Start()
    {
        pView = GetComponent<PhotonView>();
        charControl = GetComponent<CharacterController>();
        cameraController = GetComponentInChildren<CameraController>();
        anim = GetComponentInChildren<Animator>();

        speedMove = walkSpeedMove;

        if (pView.IsMine)
        {
            canvasPlayer.SetActive(false);
        }
        else
        {
            canvasPlayer.SetActive(true);
            usernameText.text = pView?.Owner.NickName;
        }
    }

    void Update()
    {
        if (!pView.IsMine)
            return;

        UpdateMovement();

        Updateinput();

        if (UpdateOnGround())
        {
            canDoubleJump = true;
        }

        UpdateAnimation();

        UpdateCrosshair();
    }

    private void UpdateMovement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 verticalMove = transform.forward * verticalInput;
        Vector3 horizontalMove = transform.right * horizontalInput;

        moveDir = (verticalMove + horizontalMove).normalized;

        charControl.Move(speedMove * Time.deltaTime * moveDir);

        transform.eulerAngles = new Vector3(0, cameraController.yaw, 0);

        ConstantGravity();

        if (verticalInput < 0)
        {
            backwards = true;
        }
        else
        {
            backwards = false;
        }

        vInput = verticalInput;

        if (isAiming)
        {
            vInput *= 0.75f;
        }

        hInput = horizontalInput;

        if (horizontalMove.magnitude > 0.1f)
        {
            strafe = true;
        }
        else
        {
            strafe = false;
        }
    }

    private void ConstantGravity()
    {
        verticalVelocity.y += gravity * Time.deltaTime;
        charControl.Move(verticalVelocity * Time.deltaTime);

        if (charControl.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -4;
        }
    }

    private bool UpdateOnGround()
    {
        return Physics.OverlapSphere(groundPosition.position, groundCheckRadius, groundLayer).Length > 0;
    }

    private void Updateinput()
    {
        if (Input.GetButtonDown("Jump") && UpdateOnGround())
        {
            verticalVelocity.y = jumpForce;
        }
        else if (Input.GetButtonDown("Jump") && canDoubleJump)
        {
            verticalVelocity.y = jumpForce;
            canDoubleJump = false;
            anim.SetTrigger("doubleJumping");
        }

        if (Input.GetButton("Fire3") && UpdateOnGround() && !isAiming)
        {
            speedMove = runSpeedMove;
            isRunning = true;
        }
        else if (!Input.GetButton("Fire3") && UpdateOnGround() && !isAiming)
        {
            speedMove = walkSpeedMove;
            isRunning = false;
        }

        if (GetComponentInChildren<WeaponController>().aiming)
        {
            isAiming = true;
            speedMove = aimingSpeedMove;
        }
        else
        {
            isAiming = false;
        }
    }

    private void UpdateAnimation()
    {
        //turn
        turnAmount = cameraController.relativeYaw;
        mouseXSmooth = Mathf.Lerp(mouseXSmooth, turnAmount, 4 * Time.deltaTime);
        anim.SetFloat("turn", mouseXSmooth * 0.5f, 0.1f, Time.deltaTime);

        //forward
        anim.SetFloat("movementAnimationSpeed", !backwards ? 1 : -1);
        anim.SetFloat("forward", Mathf.Abs(vInput), 0.1f, Time.deltaTime);

        //strafe
        anim.SetBool("strafe", strafe);
        anim.SetFloat("vertical", vInput, 0.1f, Time.deltaTime);
        anim.SetFloat("strafeAnimSpeed", isAiming ? 0.75f : 1);

        horizontalAnimSmooth = Mathf.Lerp(horizontalAnimSmooth, hInput, 2 * Time.deltaTime);
        anim.SetFloat("horizontal", horizontalAnimSmooth, 0.1f, Time.deltaTime);

        //jump
        anim.SetBool("onGround", charControl.isGrounded);
    }

    private void UpdateCrosshair()
    {
        //make a variable of a layer mask of everything
        LayerMask everything = ~0;

        RaycastHit raycastHit;
        Ray ray = cameraController.GetComponentInChildren<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

        //if the player is looking to another player, change the color of the crosshair to red, if not change to white
        if (Physics.Raycast(ray, out raycastHit, 1000, everything))
        {
            //print("ESTOY APUNTANDO A UN JUGADOR");
            if (raycastHit.collider.GetComponent<PhotonView>() != null)
            {
                if (raycastHit.collider.GetComponent<PhotonView>().IsMine)
                {
                    //print("ESTOY APUNTANDO A MI MISMO");
                    return;
                }
                
                //print("ESTE JUGADOR TIENE PHOTON VIEW");
                //call UIController change cross hair color function
                GameController.Instance.UIControllerInstance.SwitchCrossHairColor(Color.red);
            }
            else
            {
                //print("NO TIENE TIENE PHOTON VIEW");
                //call UIController change cross hair color function
                GameController.Instance.UIControllerInstance.SwitchCrossHairColor(Color.white);
            }
        }
        else
        {
            //call UIController change cross hair color function
            GameController.Instance.UIControllerInstance.SwitchCrossHairColor(Color.white);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<iTakeItem>() != null)
        {
            other.GetComponent<iTakeItem>().TakeItem();

            //check if other has coin controller or aid controller
            if (other.GetComponent<CoinController>() != null)
            {
                GetComponentInChildren<WeaponController>().AddBullets(30);
            }
            else if (other.GetComponent<AidController>() != null)
            {
                GetComponent<HealthController>().TakeDamage(-30);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (UpdateOnGround())
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawWireSphere(groundPosition.position, groundCheckRadius);
    }
}
