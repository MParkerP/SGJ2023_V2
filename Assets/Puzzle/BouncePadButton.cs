using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BouncePadButton : NetworkBehaviour
{
    [SerializeField] private GameObject bouncePad;
    [SerializeField] private float bounceForce;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            SetBouncePadForceServerRpc();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            SetBouncePadForceZeroServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetBouncePadForceServerRpc()
    {
        SetBouncePadForceClientRpc(bounceForce);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetBouncePadForceZeroServerRpc()
    {
        SetBouncePadForceClientRpc(0);
    }

    [ClientRpc]
    private void SetBouncePadForceClientRpc(float forceAmount)
    {
        if (bouncePad != null)
        {
            BouncePad bouncePadComp = bouncePad.GetComponent<BouncePad>();
            if (bouncePadComp != null)
            {
                bouncePadComp.launchForce = forceAmount;
            }
        }
    }
}
