using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class controller : MonoBehaviour
{
    internal enum driveType
    {
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }

    [SerializeField]private driveType drive;

    [Header("Variables")]

    public float kph;
    float rpm;

    public GameObject playerNamePrefab;
    public Renderer carMesh;

    public float torque = 1500;
    public float steeringMax = 30;

    Vector3 lastPosition;
    Quaternion lastRotation;
    float lastTimeMoving = 0;
    CheckpointManager cpm;

    private InputManager manager;
    public  GreatArcStudios.PauseManager pausing;
    private GameObject wheelMeshes, wheelColliders;
    private WheelCollider[] wheels = new WheelCollider[4];
    private GameObject[] wheelMesh = new GameObject[4];
    private GameObject com;
    private Rigidbody rb;
    [SerializeField] private GameObject driver;
    private Animator anim;
    private float animTurn;
    private float horizontal;
    
    private float thrust = 1000;
    private float brakePower = 50000;
    private float radius = 6;
    private float downForceValue = 100;

    public int carPrice;
    public string carName;

    public float gearLength = 3;
    public int numGears = 5;
    [HideInInspector] public int currentGear = 0;
    float currentGearPerc;
    public float lowPitch = 1f;
    public float highPitch = 6f;
    public float maxSpeed = 200;
    private bool buttonBreak = false;
    public int controlling;
    private bool ControlSet;

    [HideInInspector] public bool nitrusFlag = false;
    public ParticleSystem[] nitrusSmoke;
    public float nitrusValue;

    public Transform SkidTrailPrefab;
    Transform[] skidTrails = new Transform[4];

    public ParticleSystem smokePrefab;
    [SerializeField]ParticleSystem[] skidSmoke = new ParticleSystem[4];

    public AudioSource highAcc;
    public AudioSource skidSound;

    [Header("DEBUG")]
    public float[] slip = new float[4];

    void ResetLayer()
    {
        rb.gameObject.layer = 0;
        this.GetComponent<Ghost>().enabled = false;
    }

    // Start is called before the first frame update
    void Awake()
    {
        getObjects();

    }

    private void Start()
    {
        GameObject playerName = Instantiate(playerNamePrefab);
        playerName.GetComponent<NameUIController>().target = rb.gameObject.transform;
        this.GetComponent<Ghost>().enabled = false;

        playerName.GetComponent<Text>().text = PlayerPrefs.GetString("PlayerName");
        playerName.GetComponent<NameUIController>().carRend = carMesh;

        lastPosition = rb.gameObject.transform.position;
        lastRotation = rb.gameObject.transform.rotation;

        for (int i = 0; i < 4; i++)
        {
            skidSmoke[i] = Instantiate(smokePrefab);
            skidSmoke[i].Stop();
        }
            pausing = GameObject.Find("Pause Menu Manager").GetComponent<GreatArcStudios.PauseManager>();

        ControlSet = false;

        controlling = PlayerPrefs.GetInt("ControlScheme");
        Debug.Log(controlling);
        if(controlling == 1)
        {
            pausing.Steering();
        }
        if(controlling == 2)
        {
            pausing.JoyStick();
            Debug.Log("Hello");
        }
        if(controlling == 3)
        {
            pausing.Automatic();
        }
    }

    private void Update()
    {
        if (!RaceMonitor.racing) manager.vertical = 0;

        if(ControlSet == false)
        {
            ControlSetter();
        }

        if (cpm == null)
            cpm = rb.GetComponent<CheckpointManager>();

        if (SceneManager.GetActiveScene().name == "AwakeScene") return;
        addDownForce();
        AnimateWheels();
        MoveVehicle();
        SteerVehicle();
        if (gameObject.tag == "AI") return;
        getFriction();
        CalcEngineSound();
        CheckForSkid();
        activateNitrus();
        anim.SetFloat("turn", animTurn);

        if (controlling == 1)
        {
            manager.horizontal = SimpleInput.GetAxis("Horizontal");
        }    
        else if(controlling == 2)
        {
            manager.vertical = SimpleInput.GetAxis("Vertical");
            manager.horizontal = SimpleInput.GetAxis("Horizontal");
        }
        else if(controlling == 3)
        {
            manager.horizontal = Input.acceleration.x;

        }

        if (cpm.lap == RaceMonitor.totalLaps + 1)
        {
            highAcc.Stop();
            return;
        }

        if (rb.velocity.magnitude > 1 || !RaceMonitor.racing)
        {
            lastTimeMoving = Time.time;
        }

        RaycastHit hit;
        if (Physics.Raycast(rb.gameObject.transform.position, -Vector3.up, out hit, 10))
        {
            if (hit.collider.gameObject.tag == "road")
            {
                lastPosition = rb.gameObject.transform.position;
                lastRotation = rb.gameObject.transform.rotation;
            }

        }

        if (Time.time > lastTimeMoving + 4 || rb.gameObject.transform.position.y < -15)
        {

            rb.gameObject.transform.position = cpm.lastCP.transform.position + Vector3.up * 2;
            rb.gameObject.transform.rotation = cpm.lastCP.transform.rotation;
            rb.gameObject.layer = 8;
            this.GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }
    }

    private void SteerVehicle()
    {
        if(manager.horizontal > 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * manager.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * manager.horizontal;
            if (animTurn < 1)
                animTurn += 0.1f;
            if (animTurn == 1)
                animTurn = 1;
        }
        else if(manager.horizontal < 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * manager.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * manager.horizontal;
            if (animTurn > -1)
                animTurn -= 0.1f;
            if (animTurn == -1)
                animTurn = -1;
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;

                animTurn = 0;
        }

    }

    public void CalcEngineSound()
    {
        float gearPercentage = (1 / (float)numGears);
        float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1),
                                                    Mathf.Abs(kph / maxSpeed));
        currentGearPerc = Mathf.Lerp(currentGearPerc, targetGearFactor, Time.deltaTime * 5f);

        var gearNumFactor = currentGear / (float)numGears;
        rpm = Mathf.Lerp(gearNumFactor, 1, currentGearPerc);

        float speedPercentage = Mathf.Abs(kph / maxSpeed);
        float upperGearMax = (1 / (float)numGears) * (currentGear + 1);
        float downGearMax = (1 / (float)numGears) * currentGear;

        if (currentGear > 0 && speedPercentage < downGearMax)
            currentGear--;

        if (speedPercentage > upperGearMax && (currentGear < (numGears - 1)))
            currentGear++;

        float pitch = Mathf.Lerp(lowPitch, highPitch, rpm);
        highAcc.pitch = Mathf.Min(highPitch, pitch) * 0.25f;
    }

    private void MoveVehicle()
    {
        

        if( drive == driveType.allWheelDrive)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = manager.vertical * (torque / 4);
            }
        }
        else if(drive == driveType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = manager.vertical * (torque / 2);
            }
        }
        else
        {
            for (int i = 0; i < wheels.Length - 2; i++)
            {
                wheels[i].motorTorque = manager.vertical * (torque / 2);
            }
        }

        kph = rb.velocity.magnitude * 3.6f;
        


        /*if(manager.handbrake)
        {
            wheels[3].brakeTorque = wheels[2].brakeTorque = brakePower;
        }
        else
        {
            wheels[3].brakeTorque = wheels[2].brakeTorque = 0;
        }*/

    }

    public void OnBrakeDown()
    {
        if(buttonBreak == false)
        {
            wheels[3].brakeTorque = wheels[2].brakeTorque = brakePower;
            for (int i = 0; i < manager.brakeLight.Length; i++)
                manager.brakeLight[i].SetActive(true);

            buttonBreak = true;
        }
        
    }

    public void OnBrakeUp()
    {
        if(buttonBreak == true)
        {
            wheels[3].brakeTorque = wheels[2].brakeTorque = 0;
            for (int i = 0; i < manager.brakeLight.Length; i++)
                manager.brakeLight[i].SetActive(false);

            buttonBreak = false;
        }
        
    }

    void AnimateWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for(int i = 0; i < 4; i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }

    private void getObjects()
    {
        manager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        anim = driver.GetComponent<Animator>();
        wheelColliders = GameObject.Find("wheelColliders");
        wheelMeshes = GameObject.Find("wheelMeshes");
        wheelMesh[0] = wheelMeshes.transform.Find("0").gameObject;
        wheelMesh[1] = wheelMeshes.transform.Find("1").gameObject;
        wheelMesh[2] = wheelMeshes.transform.Find("2").gameObject;
        wheelMesh[3] = wheelMeshes.transform.Find("3").gameObject;

        wheels[0] = wheelColliders.transform.Find("0").gameObject.GetComponent<WheelCollider>(); 
        wheels[1] = wheelColliders.transform.Find("1").gameObject.GetComponent<WheelCollider>(); 
        wheels[2] = wheelColliders.transform.Find("2").gameObject.GetComponent<WheelCollider>(); 
        wheels[3] = wheelColliders.transform.Find("3").gameObject.GetComponent<WheelCollider>(); 

        com = GameObject.Find("mass");
        rb.centerOfMass = com.transform.localPosition;
    }

    private void addDownForce()
    {
        rb.AddForce(-transform.up * downForceValue * rb.velocity.magnitude);
    }
    private void getFriction()
    {
        for(int i = 0; i < wheels.Length; i++)
        {
            WheelHit wheelHit;
            wheels[i].GetGroundHit(out wheelHit);

            slip[i] = wheelHit.forwardSlip;
        }
    }

    private bool checkGears()
    {
        if (kph >= currentGear) return true;
        else return false;
    }

    public void activateNitrus()
    {
        if (!manager.boosting && nitrusValue <= 10)
        {
            nitrusValue += Time.deltaTime / 10;
        }
        else
        {
            nitrusValue -= (nitrusValue <= 0) ? 0 : Time.deltaTime / 2;

        }
        if (manager.boosting)
        {
            if (nitrusValue > 0)
            {
                startNitrusEmitter();
                rb.AddRelativeForce(Vector3.forward * thrust);
            }
            else stopNitrusEmitter();
        }
        else
            stopNitrusEmitter();
    }

    public void startNitrusEmitter()
    {
        
        if (nitrusFlag) return;
        for (int i = 0; i < nitrusSmoke.Length; i++)
        {
            nitrusSmoke[i].Play();
        }
        nitrusFlag = true;
    }

    public void stopNitrusEmitter()
    {
        if (!nitrusFlag) return;
        for (int i = 0; i < nitrusSmoke.Length; i++)
        {
            nitrusSmoke[i].Stop();
        }
        nitrusFlag = false;

    }

    public void CheckForSkid()
    {
        int numSkidding = 0;
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            wheels[i].GetGroundHit(out wheelHit);

            if (Mathf.Abs(wheelHit.forwardSlip) >= 0.4f || Mathf.Abs(wheelHit.sidewaysSlip) >= 0.4f)
            {
                numSkidding++;
                if (!skidSound.isPlaying)
                {
                    skidSound.Play();
                }
                //StartSkidTrail(i);
                skidSmoke[i].transform.position = wheels[i].transform.position - wheels[i].transform.up * wheels[i].radius;
                skidSmoke[i].Emit(1);
            }
            else
            {
                //EndSkidTrail(i);
            }
        }

        if (numSkidding == 0 && skidSound.isPlaying)
        {
            skidSound.Stop();
        }
    }

    public void StartSkidTrail(int i)
    {
        if (skidTrails[i] == null)
        {
            skidTrails[i] = Instantiate(SkidTrailPrefab);
        }

        skidTrails[i].parent = wheels[i].transform;
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
        skidTrails[i].localPosition = -Vector3.up * wheels[i].radius;
    }

    public void EndSkidTrail(int i)
    {
        if (skidTrails[i] == null) return;
        Transform holder = skidTrails[i];
        skidTrails[i] = null;
        holder.parent = null;
        holder.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(holder.gameObject, 30);
    }

    private void ControlSetter()
    {
        controlling = PlayerPrefs.GetInt("ControlScheme");
        Debug.Log(controlling);
        if (controlling == 1)
        {
            pausing.Steering();
            ControlSet = true;
        }
        if (controlling == 2)
        {
            pausing.JoyStick();
            ControlSet = true;

            Debug.Log("Hello");
        }
        if (controlling == 3)
        {
            pausing.Automatic();
            ControlSet = true;

        }
    }

}
