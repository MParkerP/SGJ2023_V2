using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WinGameCamera : NetworkBehaviour
{
    private Vector2 startingPosition = new Vector3(50.94f, 337.7f, -10);
    private Vector2 endingPosition = new Vector3(50.94f, 400.6f, -10);
    [SerializeField] private GameObject globalLightHolder;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float globalLightIntensity;
    [SerializeField] private GameObject winGameScreen;

    private bool isGameWon = false;
    private bool winStarted = false;

    private void Awake()
    {
        globalLight = globalLightHolder.GetComponent<Light2D>();
    }

    public void WinGameFunction()
    {
        if (this.GetComponent<NetworkObject>().IsOwnedByServer && !isGameWon)
        {
            if (NetworkManager.Singleton.LocalClientId != 0) { return; }
            isGameWon = true;
            FreezePlayerServerRpc();
            WinGameServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void WinGameServerRpc()
    {
        WinGameClientRpc();
    }

    [ClientRpc]
    private void WinGameClientRpc()
    {
        if (!winStarted)
        {
            StartCoroutine(WinGame());
        }
    }

    IEnumerator WinGame()
    {
        winStarted= true;
        TurnOffLaser();
        GetRidOfGhost();
        //globalLight.intensity = globalLightIntensity;
        //StartCoroutine(slideCamera());
        //yield return new WaitForSeconds(3);
        //StartCoroutine(fadeLightIn());
        //StartCoroutine(fadeLightOut());
        yield return new WaitForSeconds(5f);
        winGameScreen.SetActive(true);
        GetComponent<AudioSource>().Play();
        GameObject.Find("MusicPlayer").GetComponent<AudioSource>().Stop();
    }

    private void GetRidOfGhost()
    {
        GameObject ghost = GameObject.FindWithTag("Ghost");
        if (ghost != null)
        {
            ghost.GetComponent<Ghost>().ScareGhost();
            ghost.GetComponent<Ghost>().PlayScareAnim();
        }
        GameObject.Find("GhostSpawner").GetComponent<GhostSpawner>().isGhostSpawning = false;
    }

    IEnumerator slideCamera()
    {
        //transform.position = startingPosition;
        while (transform.position.y < endingPosition.y)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, -10);
            yield return new WaitForSeconds(.01f);
        }
        yield return new WaitForSeconds(0.01f);
    }

    IEnumerator fadeLightIn()
    {
        while (globalLight.intensity < globalLightIntensity)
        {
            globalLight.intensity += 0.01f;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.01f);
    }

    IEnumerator fadeLightOut()
    {
        while (globalLight.intensity > 0)
        {
            globalLight.intensity -= 0.01f;
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.01f);
    }

    private void TurnOffLaser()
    {
        GameObject laser = GameObject.Find("laser");
        if (laser != null)
        {
            laser.SetActive(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void FreezePlayerServerRpc()
    {
        FreezePlayerClientRpc();
    }

    [ClientRpc]
    private void FreezePlayerClientRpc()
    {
        GameObject localPlayer = GameObject.Find(NetworkManager.Singleton.LocalClientId.ToString());
        localPlayer.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
