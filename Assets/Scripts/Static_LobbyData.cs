using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Static_LobbyData
{
    public static string LobbyCode;

    public enum LobbyStatus
    {
        Creating,
        Joining
    }

    public static LobbyStatus currentLobbyStatus;
}
