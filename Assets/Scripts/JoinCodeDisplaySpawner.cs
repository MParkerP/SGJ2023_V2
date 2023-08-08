using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class JoinCodeDisplaySpawner : NetworkBehaviour
{
    [SerializeField] GameObject codeCanvas;
    public void SpawnCanvas()
    {
        Debug.Log(Static_LobbyData.LobbyCode);
        if (!Static_LobbyData.isEverythingSpawned)
        {
            Debug.Log("spawned join code yrr");
            GameObject canvas = Instantiate(codeCanvas, Vector3.zero, Quaternion.identity);
            canvas.GetComponent<NetworkObject>().Spawn();
        }

    }
}
