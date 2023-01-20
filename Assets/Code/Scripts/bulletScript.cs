using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class bulletScript : NetworkBehaviour
{
    [SerializeField] float bulletVelocity;
    [SerializeField] int bulletDamage = 25;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody projectileRB = gameObject.GetComponent<Rigidbody>();


        Vector3 forceToAdd = gameObject.transform.forward * bulletVelocity;

        projectileRB.AddForce(forceToAdd, ForceMode.Impulse);

        StartCoroutine(DestroyCoroutine());
       
    }

    /* private void FixedUpdate()
    {
        CheckHit();
    } */

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Player" && IsOwner)
        {
            var playerHit = collision.gameObject.GetComponent<NetworkObject>();
            UpdatePlayerHealthServerRpc(25, playerHit.OwnerClientId);
        }
        if (IsOwner)
        {
            DestroyBulletServerRpc();
        }

    }

    /* private void CheckHit()
    {
        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Player");

        if(Physics.Raycast(transform.position, transform.forward, out hit, 0.3f, layerMask))
        {
            Debug.DrawRay(transform.position, transform.forward *0.3f, Color.red);

            var playerHit = hit.transform.GetComponent<NetworkObject>();

            if(playerHit != null && IsOwner)
            {
                UpdatePlayerHealthServerRpc(25, playerHit.OwnerClientId);
            }
            if (IsOwner)
            {
                DestroyBulletServerRpc();
            }
        }
    } */

    [ServerRpc(RequireOwnership = false)]
    private void DestroyBulletServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(2f);

        DestroyBulletServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerHealthServerRpc(int damage, ulong clientId)
    {
        var owner = NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject;
        var playerToDamage = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerStats>();
        var playerToKill = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        if (playerToDamage != null && playerToDamage.playerHealth.Value > 0)
        {
            playerToDamage.playerHealth.Value -= damage;

        }
 

        NotifyHealthChangedClientRpc(playerToDamage.playerHealth.Value, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }

    [ClientRpc]
    public void NotifyHealthChangedClientRpc( int playerHealth, ClientRpcParams clientRpcParams = default)
    {



    }

    [ServerRpc(RequireOwnership = false)]
    public void KillAPlayerServerRpc(ulong clientId)
    {
        Debug.Log("client id to kill : " + clientId);
        

    }
}
