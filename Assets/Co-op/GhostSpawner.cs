using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

public class GhostSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject ghostPrefab;

    [SerializeField] private float laserXPos;

    [SerializeField] private GameObject leftSpawn;
    [SerializeField] private GameObject rightSpawn;

    public override void OnNetworkSpawn()
    {

    }


    public void SpawnGhostForCoop()
    {
        laserXPos = GameObject.Find("laser").transform.position.x;
        GameObject torch = GameObject.FindWithTag("Torch");

        if (torch != null)
        {
            if (torch.transform.position.x < laserXPos)
            {
                if (this.IsOwner)
                {
                    GameObject ghost = Instantiate(ghostPrefab, leftSpawn.transform.transform.position, ghostPrefab.transform.rotation);
                    ghost.GetComponent<NetworkObject>().Spawn();
                }
                
            }
            else
            {
                if (this.IsOwner)
                {
                    GameObject ghost = Instantiate(ghostPrefab, rightSpawn.transform.transform.position, ghostPrefab.transform.rotation);
                    ghost.GetComponent<NetworkObject>().Spawn();
                }
            }
        }
    }
}
