using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    [SerializeField] public NetworkVariable<int> playerHealth = new NetworkVariable<int>(100);

}
