using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootScript : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private float bulletVelocity;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePosition;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject projectile = Instantiate(bullet, firePosition.position, cam.rotation);
            Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

            Vector3 forceToAdd = cam.transform.forward * bulletVelocity;

            projectileRB.AddForce(forceToAdd, ForceMode.Impulse);
        }

    }

    
}
