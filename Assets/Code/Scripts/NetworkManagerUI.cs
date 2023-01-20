using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private TMP_InputField serverInput;
    // [SerializeField] private TMP_InputField playerNameInputField;
    // [SerializeField] private GameObject mainMenuUI;

    [SerializeField] private string serverIP = "51.38.33.24";
    [SerializeField] private int serverPORT = 4242;

    private void Awake()
    {
        hostBtn.onClick.AddListener(() => Host());
        clientBtn.onClick.AddListener(() => Client());
    }

    private void Host()
    {
        UnityTransport unityTransport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
        serverIP = "127.0.0.1";

        unityTransport.SetConnectionData(serverIP, (ushort)serverPORT);
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.StartHost();
    }

    private void Client()
    {
        UnityTransport unityTransport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

        if (serverInput.text.Length >= 1)
        {
            serverIP = serverInput.text;
        }

        Debug.Log(serverIP);

        unityTransport.SetConnectionData(serverIP, (ushort)serverPORT);
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.StartClient();
    }

    private void HandleClientConnected(ulong clientId)
    {
        gameObject.SetActive(false);
    }

    // private void AssignPlayerName()
    // {
    //     Debug.Log("playerName : " + playerNameInputField.text + "ownerId" + OwnerClientId);
    //     var playerStats = NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerStats>();
    //     playerStats.networkPlayerName.Value = playerNameInputField.text;
    //     NetworkObject.Despawn(true);
    //     Destroy(gameObject);
    //     // transform.GetChild(0).gameObject.SetActive(false);


    // }
}
