using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectController : NetworkBehaviour
{
    [SerializeField] private GameObject disconnectMessage;
    [SerializeField] private GameObject[] playersInGame;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private int playersRestartingCount = 0;
    private bool playersConnected = false;

    private void Update()
    {
        playersInGame = GameObject.FindGameObjectsWithTag("PlayerBody");
        if (playersInGame.Length == 2)
        {
            playersConnected= true;
        }

        if (playersConnected && playersInGame.Length < 2)
        {
            StartCoroutine(ForcedDisconnect());
        }

        if (playersInGame.Length == 2)
        {
            restartButton.SetActive(true);
        }
        else
        {
            restartButton.SetActive(false);
        }

        if (playersRestartingCount == 2)
        {
            RestartGame();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncreaseRestartCountServerRpc()
    {
        playersRestartingCount++;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DecreaseRestartCountServerRpc()
    {
        playersRestartingCount--;
    }

    public void RestartGame()
    {
        if(NetworkManager.Singleton.LocalClientId == 0)
        {
            /*NetworkManager.SceneManager.UnloadScene(SceneManager.GetActiveScene());
            NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);*/

            ResetPlayersServerRpc();
            DespawnDoorAndKeys();

            GameObject.FindWithTag("Torch").GetComponent<NetworkObject>().Despawn();

            NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
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

    [ServerRpc(RequireOwnership = false)]
    private void ResetPlayersServerRpc()
    {
        MovePlayerClientRpc();
    }

    [ClientRpc]
    private void MovePlayerClientRpc()
    {
        GameObject localPlayer = GameObject.Find(NetworkManager.Singleton.LocalClientId.ToString());
        if (localPlayer.GetComponent<RelativeJoint2D>() != null)
        {
            localPlayer.GetComponent<PlayerNetwork>().DropTorch();
        }

        localPlayer.transform.position = GameObject.Find("PlayerSpawnPosition").transform.position;
        localPlayer.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

    }

    IEnumerator ForcedDisconnect()
    {
        if (disconnectMessage != null)
        {
            Instantiate(disconnectMessage);
            yield return new WaitForSeconds(3);
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
            }

            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}

