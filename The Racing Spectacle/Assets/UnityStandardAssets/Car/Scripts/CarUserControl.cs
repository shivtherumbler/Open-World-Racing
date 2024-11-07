using System;
using System.Collections;
using UnityEngine;
//using UnityStandardAssets.CrossPlatformInput;
using System.IO.Ports;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        float h;
        float v;

        SerialPort serial = new SerialPort("COM4", 9600);

        private void Awake()
        {

                serial.Open();
                Debug.Log("Serial port opened successfully.");      
                StartCoroutine(ReadDataFromSerialPort());
            

            // get the car controller
            m_Car = GetComponent<CarController>();
        }

        IEnumerator ReadDataFromSerialPort()
        {
            while (true)
            {

                string[] values = serial.ReadLine ().Split(',');
                v = float.Parse(values[0]) / 100;
                h = float.Parse(values[1]) / 100;
                  
                yield return new WaitForSeconds(0.05f); // Adjust reading frequency if necessary
            }
        }
       
    }
}
