using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameOverController : NetworkBehaviour
{

    [SerializeField] private GameObject gameOverScreen;

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        FreezeTimeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void FreezeTimeServerRpc()
    {
        Time.timeScale= 0;
    }
}
