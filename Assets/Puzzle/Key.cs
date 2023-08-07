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
            DestroyKeyServerRpc();
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

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKeyServerRpc()
    {
        GameObject destroyKey = this.gameObject;
        destroyKey.GetComponent<NetworkObject>().Despawn();
    }

}
