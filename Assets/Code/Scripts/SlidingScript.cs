using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingScript : MonoBehaviour
{
    [Header("References")]
    //[SerializeField] private Transform orientation;
    [SerializeField] private Transform playerObj;
    private Rigidbody rb;
    private playerMovement pm;

    [Header("Sliding")]
    [SerializeField] float maxSlideTime;
    [SerializeField] float slideForce;
    private float slideTimer;


    [SerializeField] float slideYScale;
    private float startYScale;

    [Header("Inputs")]
    [SerializeField] private KeyCode slideInput = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private bool sliding;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<playerMovement>();

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (pm.grounded && Input.GetKeyDown(slideInput) && (horizontalInput != 0 || verticalInput != 0))
            StartSlide();
        if (pm.grounded && Input.GetKeyUp(slideInput) && sliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (sliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void StopSlide()
    {
        sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);

    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = playerObj.forward * verticalInput + playerObj.right * horizontalInput;

        if(!pm.OnSlope()|| rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        

        if (slideTimer <= 0)
            StopSlide();
    }
}
