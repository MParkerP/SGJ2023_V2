using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RestrictDoubleJump : MonoBehaviour
{
    [SerializeField] private string[] reenableJumpTags;
    [SerializeField] private GameObject playerObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (reenableJumpTags.Contains<string>(collision.gameObject.transform.tag))
        {
            playerObject.GetComponent<PlayerNetwork>().isGrounded = true;
        }
    }
}
