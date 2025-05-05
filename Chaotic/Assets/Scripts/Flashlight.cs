using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlight;

    public AudioSource turnOn;
    public AudioSource turnOff;
    
    public bool off;




    void Start()
    {
        off = true;
        flashlight.SetActive(false);
    }




    void Update()
    {
        if(off && Input.GetButtonDown("F"))
        {
            Debug.Log("Turned On");
            flashlight.SetActive(true);
            turnOn.Play();
            off = false;
        }
        else if (!off && Input.GetButtonDown("F"))
        {
            Debug.Log("Turned Off");
            flashlight.SetActive(false);
            turnOff.Play();
            off = true;
        }



    }
}
