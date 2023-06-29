using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using QFSW.QC;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;

public class TestRelay : MonoBehaviour
{
    private string joinCode;
    public string attemptedJoinCode;

    private void Awake()
    {
        ConnectToUnity();
    }


    //create a relay and generate a join code
    [Command]
    public async void CreateRelay()
    {
        try
        {
            //number of connections does not inlcude the player making the relay (1 is 2 total)
            //creates an allocation on the unity servers that can be joined
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            //output and save join code
            Debug.Log(joinCode);
            Static_LobbyData.LobbyCode = joinCode;

            //create server data from the allocation
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            //pass server data into unity relay transport
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            //start host
            NetworkManager.Singleton.StartHost();

            //load next scene using networkmanager.scenemanager
            LoadGameSceneForHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    //join a relay using a join code
    [Command]
    public async void JoinRelay()
    {
        string joinCode = attemptedJoinCode;

        if (joinCode == null)
        {
            joinCode = "";
        }

        try
        {
            Debug.Log("Joining relay with join code: " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    //async signin to unity services
    public async void ConnectToUnity()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => { Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId); };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    [Command]
    public void LoadGameSceneForHost()
    {
        var status = NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to load GameScene " +
                  $"with a {nameof(SceneEventProgressStatus)}: {status}");
        }
    }
}
