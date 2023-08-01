using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TwoPlayerTrigger : MonoBehaviour
{
    [SerializeField] private float playerCount;

    [SerializeField] public UnityEvent twoPlayerDetection;

    private void Update()
    {
        if (playerCount >= 2)
        {
            twoPlayerDetection?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerCount--;
        }
    }
}
