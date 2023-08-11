using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartGameController : NetworkBehaviour
{
    [SerializeField] private GameObject startingDoor;
    [SerializeField] private float doorSpeed;

    public void OpenDoor()
    {
        OpenDoorServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OpenDoorServerRpc()
    {
        OpenDoorClientRpc();
    }

    [ClientRpc]
    private void OpenDoorClientRpc()
    {
        StartCoroutine(slidingDoor());
    }

    IEnumerator slidingDoor()
    {
        GameObject.Find("GhostSpawner").GetComponent<GhostSpawner>().isGhostSpawning= true;
        GetComponent<AudioSource>().Play();
        startingDoor.GetComponent<Rigidbody2D>().velocity = Vector3.down.normalized * doorSpeed;
        yield return new WaitForSeconds(10);
        startingDoor.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
    }


}
