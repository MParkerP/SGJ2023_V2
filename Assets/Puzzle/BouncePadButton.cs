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
            SetBouncePadForceServerRpc(bounceForce);
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
    private void SetBouncePadForceServerRpc(float forceAmount)
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

    [ServerRpc(RequireOwnership = false)]
    private void SetBouncePadForceZeroServerRpc()
    {
        if (bouncePad != null)
        {
            BouncePad bouncePadComp = bouncePad.GetComponent<BouncePad>();
            if (bouncePadComp != null)
            {
                bouncePadComp.launchForce = 0;
            }
        }
    }

 /*   [ClientRpc]
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
    }*/
}
