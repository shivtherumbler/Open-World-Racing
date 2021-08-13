using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class awakeManager : MonoBehaviour
{
    [Header("Camera")]
    public float lerpTime;
    public GameObject CameraObject;
    public GameObject finalCameraPosition, startCameraPosition;

    [Header("Deafault Canvas")]
    public GameObject DefaultCanvas;

    [Header("Vehicle Select Canvas")]
    public GameObject vehicleSelectCanvas;
    public GameObject buyButton;
    public GameObject startButton;
    public GameObject multiplayer;
    public VehicleList listofVehicles;
    public Text currency;
    public Text carInfo;

    public GameObject toRotate;
    public float rotateSpeed;
    public int vehiclePointer = 0;
    private bool finalToStart, startToFinal;

    private void Awake()
    {
        DefaultCanvas.SetActive(true);
        vehicleSelectCanvas.SetActive(false);

        vehiclePointer = PlayerPrefs.GetInt("pointer");
        //PlayerPrefs.SetInt("currency", 5000);

        GameObject childObject = Instantiate(listofVehicles.vehicles[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
        childObject.transform.parent = toRotate.transform;
        getCarInfo();
    }

    private void FixedUpdate()
    {
        
        toRotate.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        cameraTransition();
        
    }

    public void rightButton()
    {
        if(vehiclePointer < listofVehicles.vehicles.Length - 1)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            vehiclePointer++;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject childObject = Instantiate(listofVehicles.vehicles[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
            childObject.transform.parent = toRotate.transform;
            getCarInfo();
        }
    }

    public void leftButton()
    {
        if (vehiclePointer > 0)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            vehiclePointer--;
            PlayerPrefs.SetInt("pointer", vehiclePointer);
            GameObject childObject = Instantiate(listofVehicles.vehicles[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
            childObject.transform.parent = toRotate.transform;
            getCarInfo();
        }
    }

    public void startGameButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void getCarInfo()
    {
        if(listofVehicles.vehicles[PlayerPrefs.GetInt("pointer")].GetComponent<controller>().carName.ToString() == 
            PlayerPrefs.GetString(listofVehicles.vehicles[PlayerPrefs.GetInt("pointer")].GetComponent<controller>().carName.ToString()))
        {
            carInfo.text = "Owned";
            startButton.SetActive(true);
            buyButton.SetActive(false);
            currency.text = "$" + PlayerPrefs.GetInt("currency").ToString("");


            return;
        }

        currency.text = "$" + PlayerPrefs.GetInt("currency").ToString("");


        carInfo.text = listofVehicles.vehicles[PlayerPrefs.GetInt("pointer")].GetComponent<controller>().carName.ToString() +
               " $" + listofVehicles.vehicles[PlayerPrefs.GetInt("pointer")].GetComponent<controller>().carPrice.ToString();

        startButton.SetActive(false);
        buyButton.SetActive(true);
    }
    public void buyingButton()
    {
        
        if(PlayerPrefs.GetInt("currency") >= listofVehicles.vehicles[PlayerPrefs.GetInt("pointer")].GetComponent<controller>().carPrice)
        {
            PlayerPrefs.SetInt("currency", PlayerPrefs.GetInt("currency") - listofVehicles.vehicles[PlayerPrefs.GetInt("pointer")].GetComponent<controller>().carPrice);
            PlayerPrefs.SetString(listofVehicles.vehicles[PlayerPrefs.GetInt("pointer")].GetComponent<controller>().carName.ToString(), 
                listofVehicles.vehicles[PlayerPrefs.GetInt("pointer")].GetComponent<controller>().carName.ToString());
            getCarInfo();
        }
    }

    public void DefaultCanvasStartButton()
    {
        DefaultCanvas.SetActive(false);
        vehicleSelectCanvas.SetActive(true);
        multiplayer.SetActive(false);
        startToFinal = true;
        finalToStart = false;
    }

    public void vehicleSelectCanvasStartButton()
    {
        DefaultCanvas.SetActive(true);
        vehicleSelectCanvas.SetActive(false);
        multiplayer.SetActive(true);
        finalToStart = true;
        startToFinal = false;

    }

    public void cameraTransition()
    {
        if (startToFinal)
        {
            CameraObject.transform.position = Vector3.Lerp(CameraObject.transform.position, finalCameraPosition.transform.position, lerpTime * Time.deltaTime);
        }
        if (finalToStart)
        {
            CameraObject.transform.position = Vector3.Lerp(CameraObject.transform.position, startCameraPosition.transform.position, lerpTime * Time.deltaTime);
        }

    }
}
