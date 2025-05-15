using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundManager : MonoBehaviour
{
    public AudioSource buttonClickSound;
    public AudioSource buttonHoverSound;

    public void PlayClickSound()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();
    }

    public void PlayHoverSound()
    {
        if (buttonHoverSound != null)
            buttonHoverSound.Play();
    }
}
