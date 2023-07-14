using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightLeftMovingScript : MonoBehaviour
{
    Transform platformTr;
    float newXposition;
    float leftMost;
    float rightMost;
    float speed = 1;
    int direction = 1;
    // Start is called before the first frame update
    void Start()
    {
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
                newXposition = platformTr.position.x + (0.04f * speed);
                if (platformTr.position.x > rightMost) { direction = -1; }
                break;
            case -1:
                newXposition = platformTr.position.x - (0.04f * speed);
                if (platformTr.position.x < leftMost) { direction = 1; }
                break;
        }
        platformTr.position = new Vector3(newXposition, platformTr.position.y, platformTr.position.z);
    }
}
