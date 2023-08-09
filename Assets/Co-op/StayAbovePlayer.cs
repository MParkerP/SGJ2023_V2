using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayAbovePlayer : MonoBehaviour
{
    private GameObject[] playerList;
    [SerializeField] private float holdingDistance;

    private GameObject torch;
    [SerializeField] public string side;


    private void Update()
    {
        //StayAbovePlayers();
        StayOverTorch(side);
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

    private void StayOverTorch(string side)
    {
        torch = GameObject.Find("TorchSprite");
        if (torch != null)
        {
            if (side == "left")
            {
                transform.position = new Vector2(torch.transform.position.x - holdingDistance, torch.transform.position.y + holdingDistance);
            }

            if (side == "right")
            {
                transform.position = new Vector2(torch.transform.position.x + holdingDistance, torch.transform.position.y + holdingDistance);
            }
                
        }

        
    }
}
