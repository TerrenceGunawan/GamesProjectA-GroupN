using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private Light flashlight;
    public AudioSource button;
    public bool on;

    void Start()
    {
        flashlight = GetComponent<Light>();
        on = false;
    }

    void Update()
    {
        flashlight.enabled = on;
        if(!on && Input.GetButtonDown("F"))
        {
            on = true;
            Debug.Log("Turned On");
            button.Play();
        }
        else if (on && Input.GetButtonDown("F"))
        {
            on = false;
            Debug.Log("Turned Off");
            button.Play();
        }
    }
}
