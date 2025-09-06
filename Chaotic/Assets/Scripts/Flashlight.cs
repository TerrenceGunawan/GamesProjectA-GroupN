using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private GameObject wholeBar;
    [SerializeField] private Image lightBar;
    [SerializeField] private float maxLight = 100f;
    [SerializeField] private float lightUse = 15f;
    [SerializeField] private float lightRegain = 7.5f;
    private float light;
    private PlayerActions actions;
    private InputAction flashlightAction;
    private Light flashlight;
    public AudioSource button;
    public bool On;

    void Awake()
    {
        light = maxLight;
        actions = new PlayerActions();
        flashlightAction = actions.interaction.light;
        flashlight = GetComponent<Light>();
        On = false;
    }

    public void OnEnable()
    {
        flashlightAction.Enable();
    }

    public void OnDisable()
    {
        flashlightAction.Disable();
    }

    void Update()
    {
        flashlight.enabled = On;
        if (flashlightAction.triggered)
        {
            On = !On;
            button.Play();
        }
        if(On)
        {
            wholeBar.SetActive(true);
            light -= Time.deltaTime * lightUse;
            if (light <= 0)
            {
                OnDisable();
                light = 0;
                On = false;
            }
        }
        else
        {
            light += Time.deltaTime * lightRegain;
            if (light >= maxLight)
            {
                OnEnable();
                wholeBar.SetActive(false);
                light = maxLight;
            }
        }
        float lightPercent = Mathf.Clamp01(light / maxLight);
        lightBar.fillAmount = lightPercent;
    }
}
