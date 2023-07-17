using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightLeftMovingScript : MonoBehaviour
{
    Transform platformTr;
    Rigidbody2D platformRb;
    float newXposition;
    float leftMost;
    float rightMost;
    [SerializeField] float speed = 1;
    [SerializeField] int direction = 1;
    // Start is called before the first frame update
    void Start()
    {
        platformRb = GetComponent<Rigidbody2D>();
        platformTr = GetComponent<Transform>();
        leftMost = transform.GetChild(0).GetComponent<Transform>().position.x;
        rightMost = transform.GetChild(1).GetComponent<Transform>().position.x;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        switch (direction)
        {
            case 1:
                // newXposition = platformTr.position.x + (0.04f * speed);
                platformRb.velocity = new Vector2(2*speed,0);
                if (platformTr.position.x > rightMost) { direction = -1; }
                break;
            case -1:
                // newXposition = platformTr.position.x - (0.04f * speed);
                platformRb.velocity = new Vector2(-2*speed, 0);
                if (platformTr.position.x < leftMost) { direction = 1; }
                break;
        }
       // platformTr.position = new Vector3(newXposition, platformTr.position.y, platformTr.position.z);
    }
}
