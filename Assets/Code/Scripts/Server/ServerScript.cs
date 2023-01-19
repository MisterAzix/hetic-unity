using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ServerScript : MonoBehaviour
{
    [SerializeField]
    private string serverIP = "51.38.33.24";
    [SerializeField]
    private int serverPORT = 4242;

    void Start()
    {
#if !UNITY_EDITOR && UNITY_SERVER
        UnityTransport unityTransport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(serverIP, (ushort)serverPORT);
        Debug.Log($"Setting up transport  connection to {unityTransport.ConnectionData.Address}: {unityTransport.ConnectionData.Port}");

        NetworkManager.Singleton.StartServer();
#endif
    }
}
