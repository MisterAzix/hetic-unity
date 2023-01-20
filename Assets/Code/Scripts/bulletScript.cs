using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class bulletScript : NetworkBehaviour
{
    [SerializeField] private float bulletVelocity;
    [SerializeField] private int bulletDamage = 25;

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
        Debug.Log("BulletScript - OnCollisionEnter");
        if (collision.gameObject.tag == "Player")
        {
            NetworkObject playerHit = collision.gameObject.GetComponent<NetworkObject>();
            if (OwnerClientId != playerHit.OwnerClientId)
            {
                UpdatePlayerHealthServerRpc(25, playerHit.OwnerClientId);
            }
        }

        DestroyBulletServerRpc();
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
        //var owner = NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject;
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
    public void NotifyHealthChangedClientRpc(int playerHealth, ClientRpcParams clientRpcParams = default)
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject life = FindRecursive(canvas, "Life");
        if (life)
        {
            life.GetComponent<TMP_Text>().text = playerHealth >= 0 ? playerHealth.ToString() : "0";
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void KillAPlayerServerRpc(ulong clientId)
    {
        Debug.Log("client id to kill : " + clientId);
    }

    private static GameObject FindRecursive(GameObject obj, string search)
    {
        GameObject result = null;
        foreach (Transform child in obj.transform)
        {
            if (child.name.Equals(search)) return child.gameObject;

            result = FindRecursive(child.gameObject, search);

            if (result) break;
        }

        return result;
    }
}
