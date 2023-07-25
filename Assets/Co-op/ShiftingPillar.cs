using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ShiftingPillar : NetworkBehaviour
{
    [SerializeField] private GameObject leftEndpoint;
    private Vector3 leftMaxPosition;

    [SerializeField] private GameObject rightEndpoint;
    private Vector3 rightMaxPosition;

    [SerializeField] private float moveSpeed;
    [SerializeField] private bool startRight;
    [SerializeField] private bool startLeft;

    private Rigidbody2D platformRb;

    private Vector3 currentVelocity;

    private void Start()
    {
        platformRb = GetComponent<Rigidbody2D>();


        leftMaxPosition = leftEndpoint.transform.position;
        rightMaxPosition = rightEndpoint.transform.position;

        if (startRight)
        {
            platformRb.velocity = new Vector3(1, 0, 0).normalized * moveSpeed;
            currentVelocity = platformRb.velocity;
        }
        else if (startLeft)
        {
            platformRb.velocity = new Vector3(-1, 0, 0).normalized * moveSpeed;
            currentVelocity = platformRb.velocity;
        }
    }

    private void Update()
    {
        if (transform.position.x >= rightMaxPosition.x)
        {
            transform.position = new Vector3(rightMaxPosition.x - 0.01f, transform.position.y);
            platformRb.velocity *= new Vector3(-1, 0, 0);
            currentVelocity = platformRb.velocity;
        }
        else if (transform.position.x <= leftMaxPosition.x)
        {
            transform.position = new Vector3(leftMaxPosition.x + 0.01f, transform.position.y);
            platformRb.velocity *= new Vector3(-1, 0, 0);
            currentVelocity = platformRb.velocity;
        }

        if (platformRb.velocity.magnitude < currentVelocity.magnitude || platformRb.velocity.magnitude > currentVelocity.magnitude)
        {
            platformRb.velocity = currentVelocity;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(new Vector3(leftEndpoint.transform.position.x, transform.position.y), new Vector3(rightEndpoint.transform.position.x, transform.position.y));
    }

}
