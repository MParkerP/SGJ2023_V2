using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class JoinCodeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI codeDisplay;

    public void SetJoinCodeDisplay()
    {
        Debug.Log(Static_LobbyData.LobbyCode);
        codeDisplay.text = Static_LobbyData.LobbyCode;
    }
}
