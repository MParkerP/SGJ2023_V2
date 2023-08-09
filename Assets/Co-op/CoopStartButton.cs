using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CoopStartButton : NetworkBehaviour
{
    public bool isPressed = false;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            SetBoolServerRpc(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            SetBoolServerRpc(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetBoolServerRpc(bool value)
    {
        SetBoolClientRpc(value);
    }

    [ClientRpc]
    private void SetBoolClientRpc(bool value)
    {
        isPressed = value;
    }
}
