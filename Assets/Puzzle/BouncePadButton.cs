using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BouncePadButton : MonoBehaviour
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
            SetBouncePadForceServerRpc(0);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetBouncePadForceServerRpc(float force)
    {
        SetBouncePadForceClientRpc(force);
    }

    [ClientRpc]
    private void SetBouncePadForceClientRpc(float force)
    {
        bouncePad.GetComponent<BouncePad>().launchForce = force;
    }
}
