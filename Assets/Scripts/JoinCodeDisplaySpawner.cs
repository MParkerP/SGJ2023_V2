using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class JoinCodeDisplaySpawner : NetworkBehaviour
{
    [SerializeField] GameObject codeCanvas;
    private bool isDisplayed = false;
    public void SpawnCanvas()
    {
        Debug.Log(Static_LobbyData.LobbyCode);
        if (!isDisplayed)
        {
            GameObject canvas = Instantiate(codeCanvas, Vector3.zero, Quaternion.identity);
            canvas.GetComponent<NetworkObject>().Spawn();
            isDisplayed = true;
        }

    }
}
