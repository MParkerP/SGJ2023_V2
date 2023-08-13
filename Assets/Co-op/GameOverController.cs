using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameOverController : NetworkBehaviour
{
    [SerializeField] private GameObject gameOverScreen;

    public void GameOver()
    {
        if (NetworkManager.Singleton.LocalClientId != 0) { return; }
        GameObject ghost = GameObject.FindWithTag("Ghost");
        if (ghost != null ) { ghost.GetComponent<NetworkObject>().Despawn(); }
        GameObject.Find("GhostSpawner").GetComponent<GhostSpawner>().isGhostSpawning= false;
        GameOverServerRpc();
    }

    /*    [ServerRpc(RequireOwnership = false)]
        private void FreezeTimeServerRpc()
        {
            Time.timeScale= 0;
        }*/

    [ServerRpc(RequireOwnership = false)]
    private void GameOverServerRpc()
    {
        GameOverClientRpc();
    }

    [ClientRpc]
    private void GameOverClientRpc()
    {
        
        gameOverScreen.SetActive(true);
        GetComponent<AudioSource>().Play();
        GameObject.Find("MusicPlayer").GetComponent<AudioSource>().Stop();
        GameObject localPlayer = GameObject.Find(NetworkManager.Singleton.LocalClientId.ToString());
        localPlayer.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
