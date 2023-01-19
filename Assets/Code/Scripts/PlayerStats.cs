using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    [SerializeField] public NetworkVariable<int> playerHealth = new NetworkVariable<int>(100);
    [SerializeField] private int maxPlayerHealth;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        playerHealth.OnValueChanged += HealthOnValueChanged;
    }

    private void HealthOnValueChanged(int prevHealth, int nextHealth)
    {
        Debug.Log("prevHealth : " + prevHealth + "newtHealth : " + nextHealth + "ClientId" + OwnerClientId);
        if(nextHealth == 0)
        {
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        transform.GetComponent<playerMovement>().enabled = false;
        transform.GetChild(0).GetComponent<PlayerCameraMovement>().enabled = false;
        transform.GetComponent<ShootScript>().enabled = false;
        transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(5f);
        var playerMovement = transform.GetComponent<playerMovement>();
        playerMovement.enabled = true;
        playerMovement.ResetSpawn();
        transform.GetChild(0).GetComponent<PlayerCameraMovement>().enabled = true;
        transform.GetComponent<ShootScript>().enabled = true;
        

        ResetHealthServerRpc();
       
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetHealthServerRpc()
    {
            playerHealth.Value = maxPlayerHealth;
    }


 

}
