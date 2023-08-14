using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using QFSW.QC;
using UnityEngine.Events;

public class PlayerNetwork : NetworkBehaviour
{
    //movement
    private Animator playerAn;
    private Rigidbody2D playerRb;
    [SerializeField] private SpriteRenderer playerSR;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private bool isMoving;

    [SerializeField] private Transform grabPoint;
    [SerializeField] private float grabRadius;
    [SerializeField] private LayerMask torchLayer;
    [SerializeField] private LayerMask ghostLayer;
    [SerializeField] bool isHoldingTorch = false;

    private Vector3 leftTorchPoint = new Vector3(-0.7f, 0.6f);
    private Vector3 rightTorchPoint = new Vector3(0.7f, 0.6f);
    [SerializeField] private string directionFacing = "left";

    private GameObject torchNetwork;
    private GameObject playerTorch;
    private RelativeJoint2D playerObjectJoint;
    [SerializeField] private float throwForce;

    private GameObject playerSpawnPosition;
    private bool isLookingForSpawn = true;

    [SerializeField] public bool isGrounded;
    [SerializeField] private bool isTesting;

    [SerializeField] private GameObject disconnectMessage;
    public bool isShowingDisconnect;

    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource jumpSound;


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

        //set player object name to the client id so that it can be located easily via RPC
        this.name = OwnerClientId.ToString();

        isShowingDisconnect = true;
    }

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerAn = GetComponent<Animator>();
    }

    private void Update()
    {
        //exits the update immeditately if not the owner of the object (other players)
        if (!IsOwner) return;

        //look for player spawn until finds it
        if (isLookingForSpawn)
        {
            playerSpawnPosition = GameObject.Find("PlayerSpawnPosition");
        }
        
        if (playerSpawnPosition != null && isLookingForSpawn) 
        { 
            transform.position = playerSpawnPosition.transform.position;
            isLookingForSpawn = false;
        }

        //simple horizontal movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput != 0) { isMoving = true; }
        if (isMoving) { playerRb.velocity = new Vector2(horizontalInput * speed, playerRb.velocity.y);
            playerAn.SetBool("Walking", true);
        }
        else { playerAn.SetBool("Walking", false); }
        if (horizontalInput== 0) { isMoving = false; }
        if (playerRb.velocity.y < 0 && !isGrounded) { playerAn.SetBool("Falling", true); }
        else { playerAn.SetBool("Falling", false); }
        if (playerRb.velocity.y > 0 && !isGrounded) { playerAn.SetBool("Rising", true); }
        else { playerAn.SetBool("Rising", false); }

        if (horizontalInput > 0)
        {
            TurnRight();
        }

        if (horizontalInput < 0)
        {
            TurnLeft();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            InteractWithTorch();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ThrowTorch();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isHoldingTorch)
        {
            Attack();
        }
    }

    private void Attack()
    {
        playerAn.SetTrigger("Attack");
        attackSound.PlayOneShot(attackSound.clip);
        Collider2D[] ghostsHit = Physics2D.OverlapCircleAll(grabPoint.transform.position, grabRadius, ghostLayer);
        if (ghostsHit.Length > 0)
        {
            GameObject ghost = ghostsHit[0].gameObject;
            if (ghost.GetComponent<Ghost>().isWeakToAttacks && ghost.GetComponent<Ghost>().isChasing) 
            {
                ScareGhostServerRpc("Ghost");
                GhostScaredServerRpc();
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void ScareGhostServerRpc(string ghostTag)
    {
        GameObject.FindWithTag("Ghost").GetComponent<Ghost>().ScareGhost();
        GameObject.FindWithTag("Ghost").GetComponent<Ghost>().PlayScareAnim();
    }

    [ServerRpc(RequireOwnership = false)]
    private void GhostScaredServerRpc()
    {
        GhostScaredClientRpc();
    }

    [ClientRpc]
    private void GhostScaredClientRpc()
    {
        GameObject.FindWithTag("Ghost").GetComponent<Ghost>().isScared = true;
    }

    private void Jump()
    {
        if (isTesting) { playerRb.AddForce(Vector3.up * jumpForce); }
        else
        {
            if (isGrounded)
            {
                jumpSound.PlayOneShot(jumpSound.clip);
                playerRb.AddForce(Vector3.up * jumpForce);
                isGrounded= false;
            }
        }
        
    }

    private void InteractWithTorch()
    {
        playerAn.SetTrigger("PickUp");
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
    private async void GrabTorch()
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
            playerTorch = torch;

            //set torch object to being held
            SetTorchBeingHeldBoolServerRpc(true);

            //activate any environment that needs to glow when holding torch
            ActivateGlowingEnvironment();

            //request ownership of torch from the server
            //using AWAIT in order to prevent the joint from being made before ownership is transferred
            if (!torchNetwork.GetComponent<NetworkObject>().IsOwner)
            {

                ObtainObjectOwnershipServerRpc("Torch(Clone)");
                await Task.Delay(10);
            }

            if (torchRb != null)
            {
                torchNetwork.GetComponent<Torch>().playerHoldingTorch = this.gameObject;

                //deactivate torch collider to prevent bumping into other objects
                CapsuleCollider2D torchCollider = playerTorch.GetComponent<CapsuleCollider2D>();
                torchCollider.enabled = false;

                //ensure torch does not have to float to its position and is not exerting mass to player
                if (directionFacing == "left")
                {
                    torch.transform.position = this.transform.position + leftTorchPoint;
                }
                else if (directionFacing == "right")
                {
                    torch.transform.position = this.transform.position + rightTorchPoint;
                }
                
                torch.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                torchRb.mass = 0;

                //create and set the joint before connecting to the torch
                RelativeJoint2D joint = this.AddComponent<RelativeJoint2D>();
                playerObjectJoint= joint;
                joint.enableCollision = false;
                joint.autoConfigureOffset = false;

                if (directionFacing == "left")
                {
                    joint.linearOffset = leftTorchPoint;
                }
                else if (directionFacing == "right")
                {
                    joint.linearOffset = rightTorchPoint;
                }
                joint.connectedBody = torchRb;

                //set that the player is holding the torch
                isHoldingTorch = true;
            }
        }
    }

    //player drops torch object
    public void DropTorch()
    {
        ReleaseTorch(true);
        playerTorch = null;
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

    [ServerRpc(RequireOwnership = false)]
    private void RotatePlayerSpriteServerRpc(bool isXFlipped, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        //ulong otherClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {

            var client = NetworkManager.ConnectedClients[clientId];
            client.PlayerObject.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = isXFlipped;
            RotatePlayerSpriteClientRpc(this.name, isXFlipped);
        }
    }

    [ClientRpc]
    private void RotatePlayerSpriteClientRpc(string targetName, bool value)
    {
        var targetPlayerObject = GameObject.Find(targetName);
        targetPlayerObject.GetComponent<SpriteRenderer>().flipX = value;
    }

    private void TurnLeft()
    {
        playerSR.flipX = false;
        RotatePlayerSpriteServerRpc(false);
        directionFacing = "left";

        if (playerTorch != null)
        {
            playerTorch.transform.position = this.transform.position + leftTorchPoint;
            playerObjectJoint.linearOffset = leftTorchPoint;
            playerObjectJoint.linearOffset = leftTorchPoint;
        }
    }

    private void TurnRight()
    {
        playerSR.flipX = true;
        RotatePlayerSpriteServerRpc(true);
        directionFacing = "right";

        if (playerTorch != null)
        {
            playerTorch.transform.position = this.transform.position + rightTorchPoint;
            playerObjectJoint.linearOffset = rightTorchPoint;
            playerObjectJoint.linearOffset = rightTorchPoint;
        }
    }

    private void ThrowTorch()
    {
        //ensure that player is holding the torch
        if (playerTorch == null || !isHoldingTorch) { return; }

        Vector3 throwDirection = new Vector3(0, 0, 0);

        //get throw direction depending on which way player is facing
        if (directionFacing == "left") { throwDirection = new Vector3(-1, 1).normalized; }
        else if (directionFacing == "right") { throwDirection = new Vector3(1, 1).normalized; }

        //release torch with rotating
        ReleaseTorch(true);

        //ensure that moving while throwing does not allow further throws
        playerTorch.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        //apply force to torch according to throw direction and throw force
        playerTorch.GetComponent<Rigidbody2D>().AddForce(throwDirection * throwForce);

        playerTorch = null;


    }

    //!!DOES NOT SET PLAYERTORCH TO NULL!!
    private void ReleaseTorch(bool isRotatedOnDrop)
    {
        Rigidbody2D playerTorchRb = playerTorch.GetComponent<Rigidbody2D>();

        //reactivate torch collider so it can hit other objects
        CapsuleCollider2D torchCollider = playerTorch.GetComponent<CapsuleCollider2D>();
        torchCollider.enabled = true;

        Destroy(playerObjectJoint);
        playerTorchRb.mass = 1;
        playerTorchRb.velocity *= 0.5f;

        //turn off any environment glows that are activated when holding the torch
        DeactivateGlowingEnvironment();

        //rotate the torch slightly so that it does not fall vertically
        if (isRotatedOnDrop)
        {
            if (directionFacing == "left") { playerTorch.transform.Rotate(new Vector3(0, 0, 10)); }
            else if (directionFacing == "right") { playerTorch.transform.Rotate(new Vector3(0, 0, -10)); }
        }

        torchNetwork.GetComponent<Torch>().playerHoldingTorch = null;
        SetTorchBeingHeldBoolServerRpc(false);
        isHoldingTorch = false;
    }

    //find object in scene with all children with lights that want to be on when holding the torch
    private void ActivateGlowingEnvironment()
    {
        GameObject glowingEnvironmentContainer = GameObject.Find("GlowingEnvironment");

        if (glowingEnvironmentContainer != null)
        {
            Light2D[] glowingEnvironments = glowingEnvironmentContainer.GetComponentsInChildren<Light2D>(true);

            foreach (Light2D environment in glowingEnvironments)
            {
                environment.enabled = true;
            }
        }
    }

    private void DeactivateGlowingEnvironment()
    {
        GameObject glowingEnvironmentContainer = GameObject.Find("GlowingEnvironment");

        if (glowingEnvironmentContainer != null)
        {
            Light2D[] glowingEnvironments = glowingEnvironmentContainer.GetComponentsInChildren<Light2D>(true);

            foreach (Light2D environment in glowingEnvironments)
            {
                environment.enabled = false;
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(grabPoint.transform.position, grabRadius);
    }
}
