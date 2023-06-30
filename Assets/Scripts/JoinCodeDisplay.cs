using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class JoinCodeDisplay : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI codeDisplay;

    public override void OnNetworkSpawn()
    {
        codeDisplay.text = Static_LobbyData.LobbyCode;
        base.OnNetworkSpawn();
    }
}
