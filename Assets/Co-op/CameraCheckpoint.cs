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
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float cameraShiftDelay;
    [SerializeField] private float cameraShiftInterval;
    [SerializeField] private string direction;
    //[SerializeField] private bool isMoving = false;
    [SerializeField] private GameObject cameraController;

    public int numPlayers;
    public int numPlayersFiltered;

    private void Update()
    {
        numPlayers = players.Count;
        numPlayersFiltered= playersFiltered.Count;

        playersFiltered = filterPlayerList();

        if (playersFiltered.Count >= requiredPlayers && !cameraController.GetComponent<CameraCheckpointController>().isCameraMoving)
        {
            switch (direction)
            {
                case "x":
                    cameraController.GetComponent<CameraCheckpointController>().isCameraMoving = true;
                    StartCoroutine(driftCamera(cameraPosition, "x"));
                    break;

                case "y":
                    cameraController.GetComponent<CameraCheckpointController>().isCameraMoving = true;
                    StartCoroutine(driftCamera(cameraPosition, "y"));
                    break;
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

    IEnumerator driftCamera(Vector3 targetPosition, string direction)
    {
        while (Math.Abs((mainCamera.transform.position - targetPosition).magnitude) >= 0.2f)
        {
            if (direction == "x") 
            {
                Debug.Log("attempting to move horizontally");
                if (targetPosition.x > mainCamera.transform.position.x) { mainCamera.transform.position += new Vector3(cameraShiftInterval, 0); }
                if (targetPosition.x < mainCamera.transform.position.x) { mainCamera.transform.position -= new Vector3(cameraShiftInterval, 0); }
            }

            if (direction == "y") 
            {
                Debug.Log("attempting to move vertically");
                if (targetPosition.y > mainCamera.transform.position.y) { mainCamera.transform.position += new Vector3(0, cameraShiftInterval); }
                if (targetPosition.y < mainCamera.transform.position.y) { mainCamera.transform.position -= new Vector3(0, cameraShiftInterval); }
            }

            yield return new WaitForSeconds(cameraShiftDelay);
        }
        cameraController.GetComponent<CameraCheckpointController>().isCameraMoving = false;
        yield return new WaitForSeconds(0.001f);
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
