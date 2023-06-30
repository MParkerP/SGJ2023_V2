using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;

    public void ShowTheLoadingScreen()
    {
        loadingScreen.SetActive(true);
    }
}
