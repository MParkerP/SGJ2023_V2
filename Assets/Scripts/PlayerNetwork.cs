using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerNetwork : NetworkBehaviour
{
    //movement
    private Rigidbody2D playerRb;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    [SerializeField] private Transform grabPoint;
    [SerializeField] private float grabRadius;
    [SerializeField] private LayerMask torchLayer;
    [SerializeField] private bool isHoldingTorch = false;

    private GameObject torchNetwork;

    //network variables
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //network equivalent of Awake()
    public override void OnNetworkSpawn()
    {

        //subscribe the debug function to the onvaluechanged delegate using lambda method
        randomNumber.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log(OwnerClientId + "; randomNumber: " + randomNumber.Value);
        };
    }

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //exits the update immeditately if not the owner of the object (other players)
        if (!IsOwner) return;

        //simple horizontal movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        playerRb.velocity = new Vector3(horizontalInput * speed, playerRb.velocity.y, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            InteractWithTorch();
        }
    }

    private void Jump()
    {
        playerRb.AddForce(Vector3.up * jumpForce);
    }

    private void InteractWithTorch()
    {
        if (!isHoldingTorch)
        {
            GrabTorch();
        }
        else if (isHoldingTorch)
        {
            DropTorch();
        }
    }

    //allow player to carry torch object
    private void GrabTorch()
    {
        //get the colliders from a radius around the player (should only be one)
        Collider2D[] torchesHit = Physics2D.OverlapCircleAll(grabPoint.transform.position, grabRadius, torchLayer);

        //make sure that the torch was close enough to the player
        if (torchesHit.Length > 0)
        {
            //get torch from array of colliders and access its RB
            GameObject torch = torchesHit[0].gameObject;
            Rigidbody2D torchRb = torch.GetComponent<Rigidbody2D>();
            torchNetwork = torch.transform.parent.gameObject;

            //STOP if torch already being held by other player
            if (torchNetwork.GetComponent<Torch>().isBeingHeld.Value == true)
            {
                Debug.Log("shouldnt be able to grab torch");
                return;
            }

            //request ownership of torch from the server
            //using AWAIT in order to prevent the joint from being made before ownership is transferred
            if (!torchNetwork.GetComponent<NetworkObject>().IsOwner)
            {

                ObtainObjectOwnershipServerRpc("Torch(Clone)");
            }

            if (torchRb != null)
            {
                //ensure torch does not have to float to its position and is not exerting mass to player
                torch.transform.position = this.transform.position + new Vector3(-0.75f, 0.4f);
                torchRb.mass = 0;

                //create and set the joint before connecting to the torch
                RelativeJoint2D joint = this.AddComponent<RelativeJoint2D>();
                joint.enableCollision = false;
                joint.autoConfigureOffset = false;
                joint.linearOffset = new Vector2(-0.75f, 0.4f);
                joint.connectedBody = torchRb;

                isHoldingTorch = true;
                SetTorchBeingHeldBoolServerRpc(true);
            }
        }
    }

    //player drops torch object
    private void DropTorch()
    {
        RelativeJoint2D joint = GetComponent<RelativeJoint2D>();
        GameObject torch = joint.connectedBody.gameObject;
        Rigidbody2D torchRb = torch.GetComponent<Rigidbody2D>();
        GameObject torchNetwork = torch.transform.parent.gameObject;

        Destroy(joint);
        torchRb.mass = 1;

        SetTorchBeingHeldBoolServerRpc(false);
        isHoldingTorch = false;
    }




    [ServerRpc(RequireOwnership = false)]
    private void SetTorchBeingHeldBoolServerRpc(bool status)
    {
        GameObject.Find("Torch(Clone)").GetComponent<Torch>().isBeingHeld.Value = status;
    }


    //request server to give player ownership of object by object name
    [ServerRpc(RequireOwnership = false)]
    private void ObtainObjectOwnershipServerRpc(string objectName)
    {
        GameObject gameObject = GameObject.Find(objectName);
        if (gameObject != null)
        {
            Debug.Log("found torch");
        }

        gameObject.GetComponent<NetworkObject>().ChangeOwnership(this.OwnerClientId);
    }





    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(grabPoint.transform.position, grabRadius);
    }
}
