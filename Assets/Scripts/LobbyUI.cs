using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
using Unity.Services.Lobbies;
using UnityEngine.Events;
using QFSW.QC;
using Unity.Netcode;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInput;

    [SerializeField] private TestRelay testRelay;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetAttemptedJoinCode()
    {
        testRelay.attemptedJoinCode = joinCodeInput.text;
        Static_LobbyData.LobbyCode= joinCodeInput.text;
    }

    public void SetLobbyStatus_Joining()
    {
        Static_LobbyData.currentLobbyStatus = Static_LobbyData.LobbyStatus.Joining;
    }

    public void SetLobbyStatus_Creating()
    {
        Static_LobbyData.currentLobbyStatus = Static_LobbyData.LobbyStatus.Creating;
    }


}
