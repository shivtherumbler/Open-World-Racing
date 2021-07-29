using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float vertical;
    public float horizontal;
    public bool handbrake;
    public bool boosting;
    public GameObject[] brakeLight;
    public GameObject[] displays;
    private bool firstPerson = false;
    public GameObject SwitchButton;
    public GameObject brakeButton;


    private void Start()
    {
        for (int i = 0; i < brakeLight.Length; i++)
            brakeLight[i].SetActive(false);

        displays[0] = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void FixedUpdate()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        /*handbrake = (Input.GetAxis("Jump") != 0)? true : false;
        if (Input.GetAxis("Jump") != 0)
        {
            for (int i = 0; i < brakeLight.Length; i++)
                brakeLight[i].SetActive(true);
        }
        else
        {
            for (int i = 0; i < brakeLight.Length; i++)
                brakeLight[i].SetActive(false);
        }*/
        if (Input.GetKey(KeyCode.LeftShift)) boosting = true;
        else boosting = false;

        if(Application.loadedLevelName == "AwakeScene")
        {
            SwitchButton.SetActive(false);
            brakeButton.SetActive(false);
        }
        if(this.gameObject.tag == "Player")
        {
            if (Application.loadedLevelName == "AwakeScene")
            {
                SwitchButton.SetActive(false);
                brakeButton.SetActive(false);
            }            

            else
            {
                SwitchButton.SetActive(true);
                brakeButton.SetActive(true);
            }

        }
        else
        {
            SwitchButton.SetActive(false);
            brakeButton.SetActive(false);
        }

    }

    public void FirstPerson()
    {
        
        if (firstPerson == true)
        {
            displays[0].SetActive(true);
            displays[1].SetActive(false);
            firstPerson = false;
        }

        else if (firstPerson == false)
        {
            displays[1].SetActive(true);
            displays[0].SetActive(false);
            firstPerson = true;
        }
        

    }
}
