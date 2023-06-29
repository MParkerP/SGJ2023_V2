using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    //movement
    private Rigidbody2D playerRb;
    [SerializeField] private float speed;

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
        playerRb= GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        
        //exits the update immeditately if not the owner of the object (other players)
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            TestServerRpc("This is a message");

            //randomNumber.Value = Random.Range(0, 100);
        }

        //simple horizontal movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        playerRb.velocity = new Vector3(horizontalInput * speed, playerRb.velocity.y, 0);
    }

    //send info from client to server
    [ServerRpc]
    private void TestServerRpc(string message)
    {
        Debug.Log("TestServerRpc " + OwnerClientId + "; " + message);
    }

    //send info from server to all clients
    [ClientRpc]
    private void TestClientRpc()
    {
        Debug.Log("TestClientRpc");
    }
}
