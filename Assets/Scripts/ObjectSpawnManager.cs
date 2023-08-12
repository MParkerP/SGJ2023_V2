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

        spawnGameObjects += SpawnCanvas;

        //spawn all the objects
        spawnGameObjects?.Invoke();
    }

    public void SpawnTorch()
    {
        if (this.IsOwner)// && !Static_LobbyData.isEverythingSpawned)
        {
            GameObject theTorch = Instantiate(torch, new Vector3(0, 2, 0), Quaternion.Euler(new Vector3(0,0,10)));
            theTorch.GetComponent<NetworkObject>().Spawn();
        }
    }

    public void DespawnDoorAndKeys()
    {
        var KeysAndDoors = GameObject.FindGameObjectsWithTag("KeyAndDoor");
        foreach (var key in KeysAndDoors)
        {
            key.GetComponent<NetworkObject>().Despawn();
        }
    }

    public void SpawnDoorsAndKeys()
    {
        if (this.IsOwner)// && !Static_LobbyData.isEverythingSpawned)
        {
            Debug.Log("I spawned door get owned noob");
            foreach(GameObject thing in doorsAndKeys)
            {
                GameObject theThing = Instantiate(thing);
                theThing.GetComponent<NetworkObject>().Spawn();
            }
        }

    }

    [SerializeField] GameObject codeCanvas;
    public void SpawnCanvas()
    {
        Debug.Log(Static_LobbyData.LobbyCode);
        if (this.IsOwner)// && !Static_LobbyData.isEverythingSpawned)
        {
            Debug.Log("spawned join code yrr");
            GameObject canvas = Instantiate(codeCanvas, Vector3.zero, Quaternion.identity);
            canvas.GetComponent<NetworkObject>().Spawn();
        }

    }

    public void SetObjectrsSpawned()
    {
        Static_LobbyData.isEverythingSpawned = true;
    }

    [Command]
    private void DispalyBool()
    {
        Debug.Log(Static_LobbyData.isEverythingSpawned);
    }
}
