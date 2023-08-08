using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Torch : NetworkBehaviour
{
    public NetworkVariable<bool> isBeingHeld = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private GameObject torchSpawnPosition;

    private Collider2D[] playersNearby;
    [SerializeField] private float playerRange;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float gameOverTimer;
    [SerializeField] private bool isCountdownStarted;


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.GetChild(0).transform.position, playerRange);
    }


    private void Update()
    {
        playersNearby = Physics2D.OverlapCircleAll(transform.GetChild(0).transform.position, playerRange, playerLayer);

        if (playersNearby.Length == 0)
        {
            StartCoroutine(GameOverCountdown());
        }

        if (playersNearby.Length > 0)
        {
            StopAllCoroutines();
            transform.GetChild(0).GetChild(1).GetComponent<Light2D>().intensity = 1;
            isCountdownStarted = false;
        }
    }

    IEnumerator GameOverCountdown()
    {
        
        if (!isCountdownStarted)
        {
            isCountdownStarted = true;

            Light2D torchLight = transform.GetChild(0).GetChild(1).GetComponent<Light2D>();
            while (torchLight.intensity > 0)
            {
                float delay = gameOverTimer / 20;
                torchLight.intensity -= 0.05f;
                yield return new WaitForSeconds(delay);
            }
            GameObject.Find("GameOverController").GetComponent<GameOverController>().GameOver();
        }
        
    }

    public GameObject playerHoldingTorch;
    public override void OnNetworkSpawn()
    {
        torchSpawnPosition = GameObject.Find("TorchSpawnPosition");
        if (torchSpawnPosition != null) { transform.position = torchSpawnPosition.transform.position; }
    }
}

