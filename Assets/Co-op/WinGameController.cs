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
    public bool isTriggered = false;

    public UnityEvent playerTorchDetectionCallback;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void Update()
    {
        GameObject torch = GameObject.Find("TorchSprite");
        if (torch != null)
        {
            playersDetected = Physics2D.OverlapCircleAll(transform.position, detectionRange, playerLayer);
            //torchesDetected = Physics2D.OverlapCircleAll(transform.position, detectionRange, torchLayer);

            distanceToTorch = new Vector2(GameObject.Find("TorchSprite").transform.position.x - transform.position.x, GameObject.Find("TorchSprite").transform.position.y - transform.position.y).magnitude;

            if (playersDetected.Length == 4) { isBothPlayersDetected = true; }
            else { isBothPlayersDetected = false; }

            if (distanceToTorch <= detectionRange) { isTorchDetected = true; }
            else { isTorchDetected = false; }

            if (isBothPlayersDetected && isTorchDetected && !isTriggered && !CheckCamMoving())
            {
                StopCamMoving();
                isTriggered = true;
                playerTorchDetectionCallback?.Invoke();
            }
        }
    }

    private bool CheckCamMoving()
    {
        return GameObject.Find("CameraCheckController").GetComponent<CameraCheckpointController>().isCameraMoving;
    }

    private void StopCamMoving()
    {
        var camCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        foreach (var camCheckpoint in camCheckpoints)
        {
            camCheckpoint.GetComponent<CameraCheckpoint>().StopAllCoroutines();
        }
    }
}
