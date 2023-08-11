using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundsController : MonoBehaviour
{
    [SerializeField] private AudioSource buttonHoverSound;
    [SerializeField] private AudioSource buttonPressedSound;

    public void PlayHover()
    {
        buttonHoverSound.PlayOneShot(buttonHoverSound.clip);
    }

    public void PlayPressed()
    {
        buttonPressedSound.PlayOneShot(buttonPressedSound.clip);
    }
}
