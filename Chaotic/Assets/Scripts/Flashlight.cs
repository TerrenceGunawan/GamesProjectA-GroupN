using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    private PlayerActions actions;
    private InputAction flashlightAction;
    private Light flashlight;
    public AudioSource button;
    public bool On;

    void Awake()
    {
        actions = new PlayerActions();
        flashlightAction = actions.interaction.light;
        flashlight = GetComponent<Light>();
        On = false;
    }

    void OnEnable()
    {
        flashlightAction.Enable();
    }

    void OnDisable()
    {
        flashlightAction.Disable();
    }

    void Update()
    {
        flashlight.enabled = On;
        if(flashlightAction.triggered)
        {
            On = !On;
            button.Play();
        }
    }
}
