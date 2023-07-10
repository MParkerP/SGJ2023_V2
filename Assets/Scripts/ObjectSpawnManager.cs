using System;
using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ObjectSpawnManager : NetworkBehaviour
{

    [SerializeField] private GameObject torch;
    private Action spawnGameObjects;

    public override void OnNetworkSpawn()
    {
        //add all functions that spawn objects
        spawnGameObjects += SpawnTorch;

        //spawn all the objects
        spawnGameObjects?.Invoke();
    }

    [Command]
    private void SpawnTorch()
    {
        if (this.IsOwner)
        {
            GameObject theTorch = Instantiate(torch, new Vector3(0, 2, 0), Quaternion.Euler(new Vector3(0,0,10)));
            theTorch.GetComponent<NetworkObject>().Spawn();
        }
    }
}
