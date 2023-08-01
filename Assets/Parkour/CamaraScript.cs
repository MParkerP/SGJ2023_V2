using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CamaraScript : MonoBehaviour
{
    public GameObject Player;
    public Transform playerTr;
    private Transform camaraTr;
    // Start is called before the first frame update
    void Start()
    {
        
        camaraTr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player == null)
        { 
        Player = GameObject.Find("Player(Clone)");
           // Debug.Log("Player: "+Player.name);
        }
        
       if(Player != null&&playerTr ==  null)
       {
            playerTr = Player.transform.GetChild(0).GetComponent<Transform>(); 
        }
        camaraTr.position = new Vector3(playerTr.position.x,playerTr.position.y,camaraTr.position.z);
        //Debug.Log("Player position: " + playerTr.position);
    }
}
