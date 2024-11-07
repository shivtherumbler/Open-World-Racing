using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class ArduinoInputManager : MonoBehaviour
{
    public float vertical = 0f;
    public float horizontal = 0f;

    private SerialPort serial;
    private Thread readThread;
    private bool keepReading = true;

    // Set this to your correct port and baud rate
    private string portName = "COM4"; // Change to your serial port
    private int baudRate = 9600;

    public GameObject[] brakeLight;
    public GameObject[] displays;
    [HideInInspector] public bool boosting;

    private void Start()
    {
        OpenSerialPort();
        for (int i = 0; i < brakeLight.Length; i++)
            brakeLight[i].SetActive(false);
        if (gameObject.tag == "Player")
        {
                displays[0] = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void OpenSerialPort()
    {
        // Initialize and open the serial port
        try
        {
            serial = new SerialPort(portName, baudRate);
            serial.Open();
            serial.DtrEnable = true; // Enable DTR signal
            Debug.Log("Serial port opened successfully.");

            // Start a new thread to read from the serial port
            readThread = new Thread(ReadSerialData);
            readThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to open serial port: {e.Message}");
        }
    }

    private void Update()
    {
        ReadSerialData();
        ControlCar(); // Call the method to control the car based on input
    }

    private void ReadSerialData()
    {
        if (serial.IsOpen)
        {
            try
            {
                if (serial.BytesToRead > 0)
                {
                    string line = serial.ReadLine();
                    Debug.Log($"Received Line: {line}");

                    string[] values = line.Split(',');

                    // Validate that we received the expected number of values
                    if (values.Length == 2)
                    {
                        // Attempt to parse and clamp values
                        if (float.TryParse(values[0], out float v) && float.TryParse(values[1], out float h))
                        {
                            vertical = Mathf.Clamp(v / 100f, -1f, 1f);
                            horizontal= Mathf.Clamp(h / 100f, -1f, 1f);
                            Debug.Log($"Parsed Values - Vertical: {vertical}, Horizontal: {horizontal}");
                        }
                        else
                        {
                            Debug.LogWarning("Invalid numeric data received.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Data format incorrect. Expecting two comma-separated values.");
                    }
                }
            }
            catch (TimeoutException)
            {
                // Timeout caught; can add logic if needed
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading serial data: {e.Message}");
            }
        }
    }

    private void ControlCar()
    {
        // Use the verticalInput and horizontalInput to control the car
        // Assuming you have a CarController component attached
        CarController carController = GetComponent<CarController>();
        if (carController != null)
        {
            carController.Move(horizontal, vertical, 0f, 0f); // Adjust according to your Move method's parameters
        }
    }

    private void OnApplicationQuit()
    {
        if (serial.IsOpen)
        {
            serial.Close();
            Debug.Log("Serial port closed.");
        }
    }

    public void Boosting()
    {
        boosting = true;
    }

    public void NotBoosting()
    {
        boosting = false;
    }


}
