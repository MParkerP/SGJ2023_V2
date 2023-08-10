using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class CameraCheckpoint : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 cameraPosition;

    private List<GameObject> players = new List<GameObject>();
    private List<GameObject> playersFiltered = new List<GameObject>();

    [SerializeField] private int requiredPlayers;
    [SerializeField] private float cameraShiftDelay;
    [SerializeField] private float cameraX_ShiftInterval;
    [SerializeField] private float cameraY_ShiftInterval;
    [SerializeField] private string direction;
    //[SerializeField] private bool isMoving = false;
    [SerializeField] private GameObject cameraController;
    [SerializeField] private bool isChangeSize;
    [SerializeField] private float targetCamSize;
    [SerializeField] private float growthInterval;
    [SerializeField] private float cameraGrowthDelay;
    [SerializeField] private float allowedShiftError;
    [SerializeField] private float allowedSizeError;

    public int numPlayers;
    public int numPlayersFiltered;

    private void Update()
    {
        numPlayers = players.Count;
        numPlayersFiltered= playersFiltered.Count;

        playersFiltered = filterPlayerList();

        if (playersFiltered.Count >= requiredPlayers)
        {
            if (!cameraController.GetComponent<CameraCheckpointController>().isCameraMoving)
            {
                StartCoroutine(DriftCamera(cameraPosition));
                
            }

            if (!cameraController.GetComponent<CameraCheckpointController>().isCameraGrowing)
            {
                StartCoroutine(SizeCamera(targetCamSize));
                
            }
        }
    }

    /*[ServerRpc(RequireOwnership = false)]
    private void DriftCameraServerRpc(string direction)
    {
        DriftCameraClientRpc(direction);
    }

    [ClientRpc]
    private void DriftCameraClientRpc(string direction)
    {
        StartCoroutine(driftCamera(cameraPosition, direction));
    }*/

    IEnumerator DriftCamera(Vector3 targetPosition)
    {
        cameraController.GetComponent<CameraCheckpointController>().isCameraMoving = true;
        while (Math.Abs(mainCamera.transform.position.x - targetPosition.x) > allowedShiftError || Math.Abs(mainCamera.transform.position.y - targetPosition.y) > allowedShiftError)
        {

            Debug.Log("attempting to move camera");

            if (targetPosition.x > mainCamera.transform.position.x) { mainCamera.transform.position += new Vector3(cameraX_ShiftInterval, 0); }
            if (targetPosition.x < mainCamera.transform.position.x) { mainCamera.transform.position -= new Vector3(cameraX_ShiftInterval, 0); }

            if (targetPosition.y > mainCamera.transform.position.y) { mainCamera.transform.position += new Vector3(0, cameraY_ShiftInterval); }
            if (targetPosition.y < mainCamera.transform.position.y) { mainCamera.transform.position -= new Vector3(0, cameraY_ShiftInterval); }

            yield return new WaitForSeconds(cameraShiftDelay);
        }
        cameraController.GetComponent<CameraCheckpointController>().isCameraMoving = false;
        yield return new WaitForSeconds(0.001f);
    }

    IEnumerator SizeCamera(float targetSize)
    {
        if (isChangeSize)
        {
            cameraController.GetComponent<CameraCheckpointController>().isCameraGrowing = true;
            while (Math.Abs(mainCamera.orthographicSize - targetCamSize) > allowedSizeError)
            {
                Debug.Log("attempting to size camera");
                if (mainCamera.orthographicSize < targetSize) { mainCamera.orthographicSize += growthInterval; }
                if (mainCamera.orthographicSize > targetSize) { mainCamera.orthographicSize -= growthInterval; }
                yield return new WaitForSeconds(cameraGrowthDelay);
            }

            cameraController.GetComponent<CameraCheckpointController>().isCameraGrowing = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            players.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            players.Remove(collision.gameObject);
        }
    }

    private List<GameObject> filterPlayerList()
    {
        var h = new HashSet<GameObject>(players);
        List<GameObject> a = h.ToList();

        return a;
    }
}
