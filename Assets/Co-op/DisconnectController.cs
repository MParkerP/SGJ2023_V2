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

            GameObject.FindWithTag("Torch").GetComponent<NetworkObject>().Despawn();

            NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetPlayersServerRpc()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerBody");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<RelativeJoint2D>() != null)
            {
                p.GetComponent<PlayerNetwork>().DropTorch();
            }
            p.transform.position = GameObject.Find("PlayerSpawnPosition").transform.position;
        }
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

