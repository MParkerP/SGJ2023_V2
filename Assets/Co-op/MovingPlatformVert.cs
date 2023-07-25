using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformVert : MonoBehaviour
{
    [SerializeField] private GameObject topEndpoint;
    private Vector3 topMaxPosition;

    [SerializeField] private GameObject bottomEndpoint;
    private Vector3 bottomMaxPosition;

    [SerializeField] private float moveSpeed;
    [SerializeField] private bool startUp;
    [SerializeField] private bool startDown;

    private Rigidbody2D platformRb;

    private Vector3 currentVelocity;

    private void Start()
    {
        platformRb = GetComponent<Rigidbody2D>();

        topMaxPosition = topEndpoint.transform.position;
        bottomMaxPosition = bottomEndpoint.transform.position;

        if (startUp)
        {
            platformRb.velocity = new Vector3(0, 1, 0).normalized * moveSpeed;
            currentVelocity = platformRb.velocity;
        }
        else if (startDown)
        {
            platformRb.velocity = new Vector3(0, -1, 0).normalized * moveSpeed;
            currentVelocity = platformRb.velocity;
        }
    }

    private void Update()
    {
        if (transform.position.y >= topMaxPosition.y)
        {
            transform.position = new Vector3(transform.position.x, topMaxPosition.y - 0.01f);
            platformRb.velocity *= new Vector3(0, -1, 0);
            currentVelocity = platformRb.velocity;
        }
        else if (transform.position.y <= bottomMaxPosition.y)
        {
            transform.position = new Vector3(transform.position.x, bottomMaxPosition.y + 0.01f);
            platformRb.velocity *= new Vector3(0, -1, 0);
            currentVelocity = platformRb.velocity;
        }

        if (platformRb.velocity.magnitude < currentVelocity.magnitude || platformRb.velocity.magnitude > currentVelocity.magnitude)
        {
            platformRb.velocity = currentVelocity;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(new Vector3(transform.position.x, bottomEndpoint.transform.position.y), new Vector3(transform.position.x, topEndpoint.transform.position.y));
    }
}
