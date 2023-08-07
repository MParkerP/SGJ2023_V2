using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Key : NetworkBehaviour
{
    [SerializeField] private GameObject door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBody"))
        {
            DestroyDoorServerRpc();
            this.GetComponent<NetworkObject>().Despawn();
        }
    }

/*    private void OpenDoor()
    {
        Destroy(door);
    }*/

    [ServerRpc(RequireOwnership = false)]
    private void DestroyDoorServerRpc()
    {
        GameObject destroyDoor = GameObject.Find(door.name + "(Clone)");
        destroyDoor.GetComponent<NetworkObject>().Despawn();
    }

}
