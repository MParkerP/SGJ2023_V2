using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinGameController : MonoBehaviour
{

    [SerializeField] private float detectionRange;
    [SerializeField] private Collider2D[] playersDetected;
    [SerializeField] private Collider2D[] torchesDetected;
    [SerializeField] private float distanceToTorch;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask torchLayer;

    [SerializeField] private bool isBothPlayersDetected;
    [SerializeField] private bool isTorchDetected;

    public UnityEvent playerTorchDetectionCallback;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void Update()
    {
        playersDetected = Physics2D.OverlapCircleAll(transform.position, detectionRange, playerLayer);
        //torchesDetected = Physics2D.OverlapCircleAll(transform.position, detectionRange, torchLayer);

        distanceToTorch = new Vector2(GameObject.Find("TorchSprite").transform.position.x - transform.position.x, GameObject.Find("TorchSprite").transform.position.y - transform.position.y).magnitude;

        if (playersDetected.Length == 4) { isBothPlayersDetected = true;  }
        else { isBothPlayersDetected = false; }

        if (distanceToTorch <= detectionRange) { isTorchDetected = true; }
        else { isTorchDetected = false; }

        if (isBothPlayersDetected && isTorchDetected) { playerTorchDetectionCallback?.Invoke(); }
    }
}
