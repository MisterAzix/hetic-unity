using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using TMPro;
using System;

public class PlayerStats : NetworkBehaviour
{
    [SerializeField] public NetworkVariable<int> playerHealth = new NetworkVariable<int>(100);
    [SerializeField] private int maxPlayerHealth;
    // [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] public NetworkVariable<FixedString64Bytes> networkPlayerName = new NetworkVariable<FixedString64Bytes>("");
    [SerializeField] private TMP_Text playerName;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        /* if (IsOwner)
        {
            AssignPlayerNameServerRpc();
            StartCoroutine(DestroyMenu());
        } */
        playerHealth.OnValueChanged += HealthOnValueChanged;
        // networkPlayerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HealthOnValueChanged(int prevHealth, int nextHealth)
    {
        Debug.Log($"[{DateTime.Now.ToString("HH:mm:ss\\Z")}] {OwnerClientId}: now have {nextHealth}hp!");
        //Debug.Log(OwnerClientId + "-> prevHealth : " + prevHealth + "newtHealth : " + nextHealth + "ClientId");
        if (nextHealth <= 0)
        {
            KillPlayer();
        }
    }

    private IEnumerator DestroyMenu()
    {
        yield return new WaitForSeconds(.3f);
        GameObject.Find("NetworkManagerUI").SetActive(false);

    }

    private void KillPlayer()
    {
        Debug.Log($"[{DateTime.Now.ToString("HH:mm:ss\\Z")}] {OwnerClientId}: Died!");
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

    /* [ServerRpc(RequireOwnership = false)]
    private void AssignPlayerNameServerRpc()
    {
        networkPlayerName.Value = GameObject.Find("InputName").GetComponent<TMP_InputField>().text;
        playerName.text = networkPlayerName.Value.ToString();
    } */
}
