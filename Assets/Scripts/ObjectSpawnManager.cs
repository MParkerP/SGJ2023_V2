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
    [SerializeField] private GameObject[] doorsAndKeys;
    private Action spawnGameObjects;

    public override void OnNetworkSpawn()
    {
        //add all functions that spawn objects
        spawnGameObjects += SpawnTorch;

        spawnGameObjects += SpawnDoorsAndKeys;

        //spawn all the objects
        spawnGameObjects?.Invoke();
    }

    private void SpawnTorch()
    {
        if (this.IsOwner && !Static_LobbyData.isEverythingSpawned)
        {
            GameObject theTorch = Instantiate(torch, new Vector3(0, 2, 0), Quaternion.Euler(new Vector3(0,0,10)));
            theTorch.GetComponent<NetworkObject>().Spawn();
        }
    }

    private void SpawnDoorsAndKeys()
    {
        if (this.IsOwner && !Static_LobbyData.isEverythingSpawned)
        {
            foreach(GameObject thing in doorsAndKeys)
            {
                GameObject theThing = Instantiate(thing);
                theThing.GetComponent<NetworkObject>().Spawn();
            }
        }

    }

    public void SetObjectrsSpawned()
    {
        Static_LobbyData.isEverythingSpawned = true;
    }
}
