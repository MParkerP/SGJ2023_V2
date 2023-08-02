using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ghost : NetworkBehaviour
{

    [SerializeField] private CircleCollider2D ghostRadius;
    [SerializeField] private GameObject torch;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float retreatSpeed;
    private Rigidbody2D ghostRb;

    [SerializeField] private bool isChasing;

    [SerializeField] private GameObject leftRetreat;
    [SerializeField] private GameObject rightRetreat;
    [SerializeField] private float retreatDistance;

    private bool isWeakToLasers = true;
    private bool isSearchingForRetreat = true;

    public override void OnNetworkSpawn()
    {
        torch = GameObject.Find("TorchSprite");
        ghostRb = GetComponent<Rigidbody2D>();
        leftRetreat = GameObject.Find("LeftRetreat");
        rightRetreat = GameObject.Find("RightRetreat");
    }

    private void Update()
    {
        if (torch!= null)
        {
            if (isChasing)
            {
                ChaseTorch();
            }
            else
            {
                if (isSearchingForRetreat) { Retreat(FindNearesetRetreat()); }
            }
        }

        if (!isChasing && ((transform.position.magnitude - torch.transform.position.magnitude) > retreatDistance))
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isWeakToLasers)
        {
            if (collision.CompareTag("Laser"))
            {
                ScareGhost();
            }
        }
    }

    private void ChaseTorch()
    {
        ghostRb.velocity = new Vector2(torch.transform.position.x - transform.position.x, torch.transform.position.y - transform.position.y).normalized * chaseSpeed;
    }

    private void Retreat(GameObject retreatDirection)
    {
        ghostRb.velocity = new Vector3(retreatDirection.transform.position.x, retreatDirection.transform.position.y).normalized * retreatSpeed;
        isSearchingForRetreat = false;
    }

    private GameObject FindNearesetRetreat()
    {
        float distanceToRight = new Vector3(rightRetreat.transform.position.x - transform.position.x, rightRetreat.transform.position.y - transform.position.y).magnitude;
        float distanceToLeft = new Vector3(leftRetreat.transform.position.x - transform.position.x, leftRetreat.transform.position.y - transform.position.y).magnitude;

        if (distanceToRight < distanceToLeft)
        {
            return rightRetreat;
        }
        else
        {
            return leftRetreat;
        }
    }

    private void ScareGhost()
    {
        isChasing = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(torch.transform.position, transform.position);
    }


}
