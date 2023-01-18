using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class playerMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    private float speed;

    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    private float startYScale;

    [Header("Inputs")]
    [SerializeField] private KeyCode jumpInput = KeyCode.Space;
    [SerializeField] private KeyCode sprintInput = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchInput = KeyCode.LeftControl;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public bool grounded;
    bool readyToJump;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDir;
    Rigidbody rb;

    public MovementState state;
    public enum MovementState {
        walking,
        sprinting,
        crouching,
        air
    }

    public override void OnNetworkSpawn()
    {
        readyToJump = true;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;
    }
    //void Start()
    //{
    //    if (!IsOwner) return;
    //    readyToJump = true;
    //    rb = GetComponent<Rigidbody>();
    //    rb.freezeRotation = true;

    //    startYScale = transform.localScale.y;
    //}

    void Update()
    {
        if (IsOwner && IsLocalPlayer)
        {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MyInput();
        SpeedControl();
        StateHandler();

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
        }

    }

    private void FixedUpdate()
    {
        if (IsOwner && IsLocalPlayer)
        {
        MovePlayer();
        }
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpInput) && grounded && readyToJump)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchInput))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchInput))
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    private void StateHandler()
    {

        if (grounded && Input.GetKey(crouchInput))
        {
            state = MovementState.crouching;
            speed = crouchSpeed;
        }

        else if(grounded && Input.GetKey(sprintInput) )
        {
            state = MovementState.sprinting;
            speed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            speed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        moveDir = gameObject.transform.forward * verticalInput + gameObject.transform.right * horizontalInput;
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDir) * speed * 20f, ForceMode.Force);

           
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        
        if(grounded)
            rb.AddForce(moveDir.normalized * speed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDir.normalized * speed * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {

        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > speed)
                rb.velocity = rb.velocity.normalized * speed;

        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if(flatVel.magnitude > speed)
            {
                Vector3 limitedVel = flatVel.normalized * speed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

    }

    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse );
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            if (angle < maxSlopeAngle && angle != 0)
                Debug.Log(true);
                return true;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}