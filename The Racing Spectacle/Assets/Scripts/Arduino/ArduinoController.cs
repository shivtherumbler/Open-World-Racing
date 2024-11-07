using UnityEngine;

public class ArduinoController : MonoBehaviour
{
    internal enum driveType
    {
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }

    [SerializeField] private driveType drive;
    public float torque = 1500;
    public float steeringMax = 30;
    public Rigidbody rb;
    public ArduinoInputManager manager;
    public WheelCollider[] wheels = new WheelCollider[4]; // Array of WheelColliders
    public float kph;

    private void Awake()
    {
        manager = GetComponent<ArduinoInputManager>();
        rb = GetComponent<Rigidbody>();

        // Assuming wheelColliders are set up in the Unity Editor
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i] = GameObject.Find("wheelColliders").transform.Find(i.ToString()).GetComponent<WheelCollider>();
        }
    }

    private void Update()
    {
        MoveVehicle();
        SteerVehicle();
        Debug.Log($"Motor Input: {manager.vertical}, Steering Input: {manager.horizontal}"); // Log motor and steering input
    }

    private void MoveVehicle()
    {
        float motorInput = manager.vertical; // Getting motor input

        if (drive == driveType.allWheelDrive)
        {
            foreach (WheelCollider wheel in wheels)
            {
                wheel.motorTorque = motorInput * (torque / 4);
            }
        }
        else if (drive == driveType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = motorInput * (torque / 2);
            }
        }
        else // Front wheel drive
        {
            for (int i = 0; i < wheels.Length - 2; i++)
            {
                wheels[i].motorTorque = motorInput * (torque / 2);
            }
        }
    }

    private void SteerVehicle()
    {
        float steerInput = manager.horizontal; // Getting steering input
        float steerAngle = steerInput * steeringMax;

        wheels[0].steerAngle = steerAngle;
        wheels[1].steerAngle = steerAngle;
    }
}
