using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    [HideInInspector] public float vertical;
    [HideInInspector] public float horizontal;
    [HideInInspector] public bool handbrake;
    [HideInInspector] public bool boosting;
    public GameObject[] brakeLight;
    public GameObject[] displays;
    private bool firstPerson = false;
    public GameObject SwitchButton;
    public GameObject brakeButton;
    public GameObject nitroButton;

    public Circuit way;
    public Transform currentWaypoint;

    int currentTrackerWP = 0;
    public List<Transform> circuit = new List<Transform>();
    [Range(0, 10)] public int distanceOffset = 5;
    [Range(0, 5)] public float steerForce = 1;

    [Header("AI acceleration value")]
    [Range(0, 1)] public float acceleration = 0.5f;
    public int currentNode;

    private void Start()
    {
        for (int i = 0; i < brakeLight.Length; i++)
            brakeLight[i].SetActive(false);

        if (gameObject.tag == "Player")
        {
            displays[0] = GameObject.FindGameObjectWithTag("MainCamera");
        }

        way = GameObject.FindGameObjectWithTag("circuit").GetComponent<Circuit>();
        currentWaypoint = gameObject.transform;
        circuit = way.waypoints;
    }

    private void FixedUpdate()
    {
        if (gameObject.tag == "AI") AIDrive();
        else if (gameObject.tag == "Player")
        {
            calculateDistanceOfWaypoints();
        }
        //vertical = Input.GetAxis("Vertical");
        //horizontal = Input.GetAxis("Horizontal");
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
        //if (Input.GetKey(KeyCode.LeftShift)) boosting = true;
        //else boosting = false;

        if (Application.loadedLevelName == "AwakeScene")
        {
            SwitchButton.SetActive(false);
            brakeButton.SetActive(false);
            nitroButton.SetActive(false);

        }
        if (this.gameObject.tag == "Player")
        {
            if (Application.loadedLevelName == "AwakeScene")
            {
                SwitchButton.SetActive(false);
                brakeButton.SetActive(false);
                nitroButton.SetActive(false);

            }

            else
            {
                SwitchButton.SetActive(true);
                brakeButton.SetActive(true);
                nitroButton.SetActive(true);

            }

        }
        else
        {
            SwitchButton.SetActive(false);
            brakeButton.SetActive(false);
            nitroButton.SetActive(false);

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

    public void Boosting()
    {
        boosting = true;
    }

    public void NotBoosting()
    {
        boosting = false;
    }

    private void AIDrive()
    {
        calculateDistanceOfWaypoints();
        AISteer();
        vertical = acceleration;

    }

    private void calculateDistanceOfWaypoints()
    {
        Vector3 position = gameObject.transform.position;
        float distance = Mathf.Infinity;

        for (int i = 0; i < circuit.Count; i++)
        {
            Vector3 difference = circuit[i].transform.position - position;
            float currentDistance = difference.magnitude;
            if (currentDistance < distance)
            {
                if ((i + distanceOffset) >= circuit.Count)
                {
                    currentWaypoint = circuit[1];
                    distance = currentDistance;
                }
                else
                {
                    currentWaypoint = circuit[i + distanceOffset];
                    distance = currentDistance;
                }
                currentNode = i;
            }

        }

    }

    private void AISteer()
    {

        Vector3 relative = transform.InverseTransformPoint(currentWaypoint.transform.position);
        relative /= relative.magnitude;

        horizontal = (relative.x / relative.magnitude) * steerForce;

    }
}
