using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject Canvas;

    public GameObject Car1;
    public GameObject Car2;
    public GameObject Car3;

    public Transform SpawnPos;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Canvas.SetActive(true);
        }
    }

    public void SpawnCar1()
    {
        GameObject ActiveCar = GameObject.FindGameObjectWithTag("Player");
        GameObject ActiveCam = GameObject.FindGameObjectWithTag("PlayerCamera");
        Destroy(ActiveCar);
        Destroy(ActiveCam);
        Instantiate(Car1, SpawnPos.position, SpawnPos.rotation);
    }

    public void SpawnCar2()
    {
        GameObject ActiveCar = GameObject.FindGameObjectWithTag("Player");
        GameObject ActiveCam = GameObject.FindGameObjectWithTag("PlayerCamera");
        Destroy(ActiveCar);
        Destroy(ActiveCam);
        Instantiate(Car2, SpawnPos.position, SpawnPos.rotation);
    }

    public void SpawnCar3()
    {
        GameObject ActiveCar = GameObject.FindGameObjectWithTag("Player");
        GameObject ActiveCam = GameObject.FindGameObjectWithTag("PlayerCamera");
        Destroy(ActiveCar);
        Destroy(ActiveCam);
        Instantiate(Car3, SpawnPos.position, SpawnPos.rotation);
    }



}
