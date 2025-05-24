using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private Light flashlight;
    public AudioSource button;
    public bool On;

    void Start()
    {
        flashlight = GetComponent<Light>();
        On = false;
    }

    void Update()
    {
        flashlight.enabled = On;
        if(!On && Input.GetButtonDown("F"))
        {
            On = !On;
            button.Play();
        }
        else if (On && Input.GetButtonDown("F"))
        {
            On = !On;
            button.Play();
        }
    }
}
