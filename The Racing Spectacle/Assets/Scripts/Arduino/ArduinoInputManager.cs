using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public bool boosting = false;
    public bool braking = false;

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
        if(SceneManager.GetActiveScene().name == "ArduinoTest")
        {
            // Check if the port is available before trying to open it
            if (SerialPort.GetPortNames().Contains(portName))
            {
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
                catch (UnauthorizedAccessException e)
                {
                    Debug.LogError($"Access to the port {portName} is denied. {e.Message}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to open serial port: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"Port {portName} is not available.");
            }
        }
    }

    private void Update()
    {
        // Ensure the serial data is read in a non-blocking way in the background thread
        // You can also put a small delay in the Update method if it's calling ReadSerialData too often
        ControlCar(); // Call the method to control the car based on input
    }

    private void ReadSerialData()
    {
        while (keepReading)
        {
            if (serial.IsOpen)
            {
                try
                {
                    string line = serial.ReadLine();
                    Debug.Log($"Received Line: {line}");

                    string[] values = line.Split(',');

                    // Ensure we have exactly four values (speed, rotation, boost, brake)
                    if (values.Length == 4 &&
                        float.TryParse(values[0], out float v) &&
                        float.TryParse(values[1], out float h) &&
                        int.TryParse(values[2], out int boost) &&
                        int.TryParse(values[3], out int brake))
                    {
                        vertical = Mathf.Clamp(v / 100f, -1f, 1f);
                        horizontal = Mathf.Clamp(h / 100f, -1f, 1f);
                        boosting = boost == 1;
                        braking = brake == 1;
                        Debug.Log($"Parsed Values - Vertical: {vertical}, Horizontal: {horizontal}, Boosting: {boosting}, Braking: {braking}");
                    }
                    else
                    {
                        Debug.LogWarning("Data format incorrect or invalid numeric data received.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error reading serial data: {e.Message}");
                }
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
            // Adjust car control based on boost and brake states
            float adjustedVertical = vertical;
            float brakeInput = braking ? 1f : 0f;

            // Apply boost effect
            if (boosting)
            {
                adjustedVertical *= 1.5f; // Increase speed by 50% during boost
            }

            carController.Move(horizontal, adjustedVertical, adjustedVertical, brakeInput);
        }
    }

    private void OnDestroy()
    {
        // Ensure the serial port is closed properly when the application quits
        if (serial.IsOpen)
        {
            serial.Close();
            Debug.Log("Serial port closed.");
        }

        // Stop the read thread
        keepReading = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join(); // Wait for the thread to finish
        }
    }
}
