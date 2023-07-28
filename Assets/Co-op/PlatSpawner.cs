using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject glowingParent;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            GameObject currentPlat = Instantiate(platform);
            NetworkObject net_currentPlat = currentPlat.GetComponent<NetworkObject>();

            net_currentPlat.Spawn();
        }

        
    }
}
