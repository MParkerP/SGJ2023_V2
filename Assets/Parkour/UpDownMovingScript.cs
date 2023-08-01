using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownMovingScript : MonoBehaviour
{
    Transform platformTr;
    float newYposition;
    float minHeight;
    float maxHeight;
    [SerializeField] float speed = 1;
    [SerializeField] int direction = 1;
    private bool isMoving = false;
    public GameObject secondPlayer;
    // Start is called before the first frame update
    void Start()
    {
     platformTr = GetComponent<Transform>();
     minHeight = transform.GetChild(0).GetComponent<Transform>().position.y;
     maxHeight = transform.GetChild(1).GetComponent<Transform>().position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
          secondPlayer = GameObject.Find("1");
        }
        if (secondPlayer != null) 
        { 
            isMoving = true;
        }
    }

    private void FixedUpdate()
    {
        
        if (isMoving)
        {
            switch (direction)
            {
                case 1:
                    newYposition = platformTr.position.y + (0.04f * speed);
                    if (platformTr.position.y > maxHeight) { direction = -1; }
                    break;
                case -1:
                    newYposition = platformTr.position.y - (0.04f * speed);
                    if (platformTr.position.y < minHeight) { direction = 1; }
                    break;
            }
            platformTr.position = new Vector3(platformTr.position.x, newYposition, platformTr.position.z);
        }
    }
}
