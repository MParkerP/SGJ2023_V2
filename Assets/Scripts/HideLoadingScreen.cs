using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideLoadingScreen : MonoBehaviour
{

    [SerializeField] private GameObject LoadingScreen;

    public void HideTheLoadingScreen()
    {
        if (LoadingScreen.activeSelf)
        {
            LoadingScreen.SetActive(false);
        }
        
    }
}
