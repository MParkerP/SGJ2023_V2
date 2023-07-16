using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatScript : MonoBehaviour
{
    Transform platformTr;
    Rigidbody2D platformRb;
    Vector3 startPosition;
    [SerializeField] int timeBeforeFall = 10;
    [SerializeField] int respawnTime = 60;
    int platTime;
    bool fall = false;
    // Start is called before the first frame update
    void Start()
    {
        platformTr = GetComponent<Transform>(); 
        platformRb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        platTime = timeBeforeFall;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
       fall = true;
    }

    private void FixedUpdate()
    {
        if (fall) { platTime--; }
       
    }
    // Update is called once per frame
    void Update()
    {
        if (platTime <= 0 && platformRb.bodyType != RigidbodyType2D.Dynamic)
        {
            platformRb.bodyType = RigidbodyType2D.Dynamic;
        }
        else if (platTime < respawnTime*-1)
        {
            platformRb.bodyType = RigidbodyType2D.Static;
            platformTr.position = startPosition;
            platTime = timeBeforeFall;
            fall = false;
        }
    }
}
