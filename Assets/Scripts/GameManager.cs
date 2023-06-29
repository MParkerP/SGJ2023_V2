using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private UnityEvent onGameSceneLoaded;

    private void Awake()
    {
        //when the game manager wakes, add the event onGameSceneLoaded to the callback for the network onloadeventcompleted delegate
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted +=
                (string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) => { onGameSceneLoaded?.Invoke(); };
    }
}
