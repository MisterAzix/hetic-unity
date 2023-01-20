using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootScript : NetworkBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePosition;

    // Update is called once per frame
    void Update()
    {
        if (IsOwner && Input.GetMouseButtonDown(0))
        {
            ShootServerRpc();
        }
    }
  
    [ServerRpc]
    private void ShootServerRpc()
    {
        Debug.Log($"[{DateTime.Now.ToString("HH:mm:ss\\Z")}] {OwnerClientId}: Shoot!");
        GameObject projectile = Instantiate(bullet, firePosition.position, cam.rotation);
        NetworkObject projectileNetworkObject = projectile.GetComponent<NetworkObject>();
        projectileNetworkObject.Spawn(true);
        if (projectileNetworkObject.OwnerClientId != OwnerClientId)
        {
            projectileNetworkObject.ChangeOwnership(OwnerClientId);
        }
    }
}
