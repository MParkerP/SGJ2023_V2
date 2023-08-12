using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] private GameObject bounceDirection;
    [SerializeField] public float launchForce;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(this.transform.position, bounceDirection.transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float bounceX = bounceDirection.transform.position.x;
        float bounceY = bounceDirection.transform.position.y;
        float padX = transform.position.x;
        float padY = transform.position.y;

        if (collision.rigidbody && (collision.gameObject.CompareTag("PlayerBody") || collision.gameObject.CompareTag("Torch")))
        {
            PlaySound();
            Vector3 launchDirection = new Vector3(bounceX - padX, bounceY - padY);
            collision.rigidbody.AddForce(launchDirection.normalized * launchForce);
        }
    }

    private void PlaySound()
    {
        GameObject soundMaker = GameObject.Find("BouncePadSounds");
        if (soundMaker != null)
        {
            soundMaker.GetComponent<AudioSource>().PlayOneShot(soundMaker.GetComponent<AudioSource>().clip);
        }
    }
}
