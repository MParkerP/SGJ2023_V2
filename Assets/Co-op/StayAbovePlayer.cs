using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayAbovePlayer : MonoBehaviour
{
    private GameObject[] playerList;
    [SerializeField] private float holdingDistance;


    private void Update()
    {
        StayAbovePlayers();
    }

    private float getAveragerPlayerY()
    {
        float totalY = 0;
        foreach (GameObject player in playerList)
        {
            totalY += player.transform.position.y;
        }

        float averageY = totalY/ playerList.Length;

        return averageY;
    }
    
    private void StayAbovePlayers()
    {
        playerList = GameObject.FindGameObjectsWithTag("PlayerBody");
        if (playerList.Length > 0)
        {
            transform.position = new Vector2(transform.position.x, getAveragerPlayerY() + holdingDistance);
        }

    }
}
