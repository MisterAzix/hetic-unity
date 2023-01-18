using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCameraMovement : NetworkBehaviour
{
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;
    [SerializeField] private Transform player;
    [SerializeField] private Transform gunPosition;

    float xRotation;
    float yRotation;


    public override void OnNetworkSpawn()
    {
        if(IsOwner && IsLocalPlayer)
        {
        if(gameObject.GetComponent<Camera>().enabled == false)
            gameObject.GetComponent<Camera>().enabled = true;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    //private void Start()
    //{

    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;
    //}

    private void Update()
    {
        if (IsOwner && IsLocalPlayer)
        {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        player.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        gunPosition.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        }

    }
}
