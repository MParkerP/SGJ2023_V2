using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject ghostPrefab;

    [SerializeField] private float laserXPos;

    [SerializeField] private GameObject leftSpawn;
    [SerializeField] private GameObject rightSpawn;

    [SerializeField] public bool isGhostSpawning = false;
    [SerializeField] private float spawnDelay;

    public override void OnNetworkSpawn()
    {
        StartCoroutine(GhostSpawning());
    }

    public void StartGhostSpawning()
    {
        StartCoroutine(GhostSpawning());
    }

    IEnumerator GhostSpawning()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnGhostForCoop();
    }

    public void SpawnGhostForCoop()
    {
        if (NetworkManager.Singleton.LocalClientId!=0) { return; }
        if (!isGhostSpawning) { return; }

        GameObject laser = GameObject.Find("laser");
        if (laser != null) { laserXPos = laser.transform.position.x; }
        GameObject torch = GameObject.Find("TorchSprite");

        if (torch != null && laser != null)
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
        else
        {
            int spawnPosition = Random.Range(0, 2);
            GameObject spawnPlace = null;

            switch (spawnPosition)
            {
                case 0:
                    spawnPlace = leftSpawn;
                    break;
                case 1: 
                    spawnPlace = rightSpawn;
                    break;
            }

            GameObject ghost = Instantiate(ghostPrefab, spawnPlace.transform.transform.position, ghostPrefab.transform.rotation);
            ghost.GetComponent<NetworkObject>().Spawn();
        }
    }
}
