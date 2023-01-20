using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using TMPro;
using System;

public class PlayerStats : NetworkBehaviour
{
    [SerializeField] private int maxPlayerHealth;
    [SerializeField] private int respawnTime = 5;
    // [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_Text playerName;

    [SerializeField] public NetworkVariable<int> playerHealth = new NetworkVariable<int>(100);
    [SerializeField] public NetworkVariable<FixedString64Bytes> networkPlayerName = new NetworkVariable<FixedString64Bytes>("");

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        /* if (IsOwner)
        {
            AssignPlayerNameServerRpc();
        } */

        playerHealth.OnValueChanged += HealthOnValueChanged;
        // networkPlayerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HealthOnValueChanged(int prevHealth, int nextHealth)
    {
        if (nextHealth <= 0)
        {
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        Debug.Log($"[{DateTime.Now.ToString("HH:mm:ss\\Z")}] {OwnerClientId}: Died!");
        ToggleRespawnUI();

        transform.GetComponent<playerMovement>().enabled = false;
        transform.GetChild(0).GetComponent<PlayerCameraMovement>().enabled = false;
        transform.GetComponent<ShootScript>().enabled = false;
        transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        int counter = respawnTime;
        UpdateRespawnTimer(counter);
        while (counter > 0)
        {
            yield return new WaitForSeconds(1f);
            counter--;
            UpdateRespawnTimer(counter);
        }
        ToggleRespawnUI();

        var playerMovement = transform.GetComponent<playerMovement>();
        playerMovement.enabled = true;
        playerMovement.ResetSpawn();
        transform.GetChild(0).GetComponent<PlayerCameraMovement>().enabled = true;
        transform.GetComponent<ShootScript>().enabled = true;

        ResetHealthServerRpc(OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetHealthServerRpc(ulong clientId)
    {
        playerHealth.Value = maxPlayerHealth;

        NotifyHealthChangedClientRpc(maxPlayerHealth, new ClientRpcParams
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

    private void UpdateRespawnTimer(int counter)
    {
        if (IsOwner && IsLocalPlayer)
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject respawnTimer = FindRecursive(canvas, "RespawnTimer");
            if (respawnTimer)
            {
                respawnTimer.GetComponent<TMP_Text>().text = $"Respawn in {counter}...";
            }
        }
    }

    private void ToggleRespawnUI()
    {
        if (IsOwner && IsLocalPlayer)
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject gameUI = FindRecursive(canvas, "GameUI");
            GameObject deathUI = FindRecursive(canvas, "DeathUI");

            if (gameUI && deathUI)
            {
                gameUI.SetActive(!gameUI.activeSelf);
                deathUI.SetActive(!deathUI.activeSelf);
            }
        }
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

    /* [ServerRpc(RequireOwnership = false)]
    private void AssignPlayerNameServerRpc()
    {
        networkPlayerName.Value = GameObject.Find("InputName").GetComponent<TMP_InputField>().text;
        playerName.text = networkPlayerName.Value.ToString();
    } */
}
