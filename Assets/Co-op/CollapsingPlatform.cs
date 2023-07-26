using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsingPlatform : MonoBehaviour
{
    [SerializeField] private SpriteRenderer platformRenderer;
    [SerializeField] private List<BoxCollider2D> platformColliders;

    [SerializeField] private float flashInterval;
    [SerializeField] private float flashDuration;
    [SerializeField] private float platDownTime;

    private bool isCollapsing;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(!isCollapsing) { StartCoroutine(CollapsePlatform()); }
        }
    }

    IEnumerator CollapsePlatform()
    {
        isCollapsing = true;

        //save the color of the platform to reference it 
        Color currentPlatColor = platformRenderer.color;

        //make a copy of the color with an alpha of zero
        Color tempInvis = platformRenderer.color;
        tempInvis.a = 0f;

        //flash the sprite color to white three times
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(flashInterval);
            platformRenderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
            platformRenderer.color = currentPlatColor;
        }

        //make sprite invisible
        platformRenderer.color = tempInvis;

        //disable all colliders on the platform
        foreach (Collider2D platCollider in platformColliders)
        {
            platCollider.enabled = false;
        }

        yield return new WaitForSeconds(platDownTime);

        //reset color and colliders on the platform
        platformRenderer.color = currentPlatColor;
        foreach (Collider2D platCollider in platformColliders)
        {
            platCollider.enabled = true;
        }

        isCollapsing = false;
    }
}
