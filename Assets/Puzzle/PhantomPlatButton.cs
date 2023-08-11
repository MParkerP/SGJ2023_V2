using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PhantomPlatButton : NetworkBehaviour
{
    [SerializeField] private GameObject[] phantomPlats;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBody") && collision.gameObject.transform.parent.GetComponent<NetworkObject>().IsLocalPlayer)
        {
            TogglePhantomPlatsServerRpc();
        }
    }

/*    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            TogglePhantomPlatsServerRpc();
        }
    }*/

    [ServerRpc(RequireOwnership = false)]
    private void TogglePhantomPlatsServerRpc()
    {
        TogglePhantomPlatsClientRpc();
    }

    [ClientRpc]
    private void TogglePhantomPlatsClientRpc()
    {
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
        foreach(GameObject phantomPlat in phantomPlats)
        {
            Collider2D[] platColliders = phantomPlat.GetComponentsInChildren<Collider2D>();
            foreach(Collider2D platCollider in platColliders)
            {
                platCollider.enabled = !platCollider.enabled;
            }

            SpriteRenderer platRend = phantomPlat.GetComponent<SpriteRenderer>();
            Color tempColor = platRend.color;

            if (platRend.color.a == 0)
            {
                tempColor.a = 1;
            }
            else
            {
                tempColor.a = 0;
            }

            platRend.color = tempColor;
        }
    }
}
