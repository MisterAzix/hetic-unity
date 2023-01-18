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
        if (Input.GetMouseButtonDown(0))
        {
            if (IsOwner)
            {
                ShootServerRpc();
            }

           
        }

    }
  

    [ServerRpc]
    private void ShootServerRpc()
    {
        GameObject projectile = Instantiate(bullet, firePosition.position, cam.rotation);
        projectile.GetComponent<NetworkObject>().Spawn(true);
    }

   

}
