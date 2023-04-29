using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor.Rendering;

public class PlayerController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float walkSpeedMove = 4f;
    [SerializeField] private float runSpeedMove = 6f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float gravity = -14f;
    bool isRunning;

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
        }

        if (Input.GetButton("Fire3") && UpdateOnGround())
        {
            speedMove = runSpeedMove;
            isRunning = true;
        }
        else if (!Input.GetButton("Fire3") && UpdateOnGround())
        {
            speedMove = walkSpeedMove;
            isRunning = false;
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

        horizontalAnimSmooth = Mathf.Lerp(horizontalAnimSmooth, hInput, 2 * Time.deltaTime);
        anim.SetFloat("horizontal", horizontalAnimSmooth, 0.1f, Time.deltaTime);

        ////jump
        //anim.SetBool("IsGrounded", UpdateOnGround());
        //anim.SetBool("IsRunning", isRunning);
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
