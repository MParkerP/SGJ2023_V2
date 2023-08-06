using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Ghost : NetworkBehaviour
{

    [SerializeField] private CircleCollider2D ghostRadius;
    [SerializeField] private GameObject torch;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float retreatSpeed;
    private Rigidbody2D ghostRb;

    [SerializeField] public bool isChasing;

    [SerializeField] private GameObject leftRetreat;
    [SerializeField] private GameObject rightRetreat;
    [SerializeField] private float retreatDistance;

    private bool isWeakToLasers = true;
    private bool isSearchingForRetreat = true;
    private bool isHoldingTorch = false;
    private Vector3 torchHoldingPoint = new Vector3(0, 0, 0);

    [SerializeField] private float grabRange;
    [SerializeField] private LayerMask torchLayer;
    private float distanceToTorch;
    public bool isWeakToAttacks;
    private Animator ghostAn;

    public override void OnNetworkSpawn()
    {
        ghostAn = GetComponent<Animator>();
        torch = GameObject.Find("TorchSprite");
        ghostRb = GetComponent<Rigidbody2D>();
        leftRetreat = GameObject.Find("LeftRetreat");
        rightRetreat = GameObject.Find("RightRetreat");
    }

    private void Update()
    {
        if (isHoldingTorch)
        {
            torch.transform.position = transform.position;
        }

        if (torch!= null)
        {
            if (isChasing)
            {
                ChaseTorch();
            }
            else
            {
                if (isSearchingForRetreat) { Retreat(); }
            }
        }

        if (!isChasing && ((transform.position.magnitude - torch.transform.position.magnitude) > retreatDistance))
        {
            GetComponent<NetworkObject>().Despawn();
        }

        distanceToTorch = new Vector3(transform.position.x - torch.transform.position.x, transform.position.y - torch.transform.position.y).magnitude;
        if (distanceToTorch < grabRange && !isHoldingTorch)
        {
            StealTorch();
            ScareGhost();
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

        if (!isHoldingTorch)
        {
            if (collision.CompareTag("Torch"))
            {
                StealTorch();
                ScareGhost();
            }
        }

    }

    private void ChaseTorch()
    {
        ghostRb.velocity = new Vector2(torch.transform.position.x - transform.position.x, torch.transform.position.y - transform.position.y).normalized * chaseSpeed;
    }

    private void Retreat()
    {
        GameObject nearestRetreat = FindNearesetRetreat();
        float laserXPos = GameObject.Find("laser").transform.position.x;
        if (nearestRetreat.transform.position.x > laserXPos)
        {
            ghostRb.velocity = new Vector2(1,1).normalized * retreatSpeed;
        }
        else
        {
            ghostRb.velocity = new Vector2(-1,-1).normalized * retreatSpeed;
        }
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

    public void ScareGhost()
    {
        isChasing = false;
    }

    private void StealTorch()
    {
        ghostAn.SetTrigger("Lick");
        if (torch.transform.parent.gameObject.GetComponent<Torch>().playerHoldingTorch != null)
        {
            GameObject playerHoldingTorch = torch.transform.parent.gameObject.GetComponent<Torch>().playerHoldingTorch;
            playerHoldingTorch.GetComponent<PlayerNetwork>().DropTorch();
        }

        SetTorchBeingHeldBoolServerRpc(true);
        CapsuleCollider2D torchCollider = torch.GetComponent<CapsuleCollider2D>();
        Rigidbody2D torchRb = torch.GetComponent<Rigidbody2D>();

        if (torch.transform.parent.gameObject.GetComponent<NetworkObject>().OwnerClientId!=0)
        {
            GiveTorchToHostServerRpc();
            SetTorchPositionServerRpc();
        }

        torchCollider.enabled = false;
        torch.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        torch.transform.position = transform.position + torchHoldingPoint;
        torchRb.mass = 0;

        RelativeJoint2D joint = this.AddComponent<RelativeJoint2D>();
        joint.enableCollision = false;
        joint.autoConfigureOffset = false;
        joint.linearOffset = torchHoldingPoint;
        joint.connectedBody = torchRb;

        isHoldingTorch = true;
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void GiveTorchToHostServerRpc()
    {
        torch.transform.parent.gameObject.GetComponent<NetworkObject>().ChangeOwnership(0);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetTorchPositionServerRpc()
    {
        SetTorchPositionClientRpc();
    }

    [ClientRpc]
    private void SetTorchPositionClientRpc()
    {
        torch.transform.transform.position = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(torch.transform.position, transform.position);
        Gizmos.DrawSphere(transform.position, grabRange);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetTorchBeingHeldBoolServerRpc(bool status)
    {
        GameObject.Find("Torch(Clone)").GetComponent<Torch>().isBeingHeld.Value = status;
    }


}
