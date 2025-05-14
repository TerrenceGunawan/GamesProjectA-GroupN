using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 


public class Keypad : MonoBehaviour
{
    public Player player;
    public GameObject keypadOB;
    public GameObject animateOB;
    public Animator ANI;


    public TextMeshProUGUI textOB;
    public string answer = "12345";
    public bool Right;

    public AudioSource button;
    public AudioSource correct;
    public AudioSource wrong;

    public bool animate;


    void Start()
    {
        keypadOB.SetActive(false);
        Right = false;
    }


    public void Number(int number)
    {
        textOB.text += number.ToString();
        Right = true;
        // button.Play();
    }

    public void Execute()
    {
        if (textOB.text == answer)
        {
            // correct.Play();
            textOB.text = "Right";
            Exit();

        }
        else
        {
            // wrong.Play();
            textOB.text = "Wrong";
        }


    }

    public void Clear()
    {
        {
            textOB.text = "";
            button.Play();
        }
    }

    public void Exit()
    {
        keypadOB.SetActive(false);
        player.EnableMovement();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        if (textOB.text == "Right" && animate)
        {
            ANI.SetBool("animate", true);
            Debug.Log("its open");
        }


        if(keypadOB.activeInHierarchy)
        {
            player.DisableMovement();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }


}