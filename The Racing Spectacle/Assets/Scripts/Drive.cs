using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drive : MonoBehaviour
{

    public WheelCollider[] WCs;
    public GameObject[] Wheels;
    public float torque = 200;
    public float maxSteerAngle = 30;
    public float maxBrakeTorque = 5000;
    [HideInInspector] public bool nitrusFlag = false;
    [HideInInspector] public float nitrusValue;
    [HideInInspector] public float KPH;
    private float radius = 6;
    public ParticleSystem[] nitrusSmoke;

    public GameObject playerNamePrefab;
    public Renderer carMesh;

    public AudioSource skidSound;
    public AudioSource highAcc;

    public Transform SkidTrailPrefab;
    Transform[] skidTrails = new Transform[4];

    public ParticleSystem smokePrefab;
    ParticleSystem[] skidSmoke = new ParticleSystem[4];

    public GameObject brakeLight;

    public Rigidbody rb;
    public float gearLength = 3;
    public float currentSpeed { get { return rb.velocity.magnitude * gearLength; } }
    public float lowPitch = 1f;
    public float highPitch = 6f;
    public int numGears = 5;
    float rpm;
    [HideInInspector] public int currentGear = 1;
    float currentGearPerc;
    public float maxSpeed = 200;

    string[] aiNames = { "Mark", "Axel", "Nathan", "Shivansh", "John", "Rocky", "Shawn" };

    public void StartSkidTrail(int i)
    {
        if(skidTrails[i] == null)
        {
            skidTrails[i] = Instantiate(SkidTrailPrefab);
        }

        skidTrails[i].parent = WCs[i].transform;
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
        skidTrails[i].localPosition = -Vector3.up * WCs[i].radius;
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

    // Start is called before the first frame update
    void Start()
    {
        for(int i =0; i<4;i++)
        {
            skidSmoke[i] = Instantiate(smokePrefab);
            skidSmoke[i].Stop();
        }

        brakeLight.SetActive(false);

        GameObject playerName = Instantiate(playerNamePrefab);
        playerName.GetComponent<NameUIController>().target = rb.gameObject.transform;

        if (this.GetComponent<AISystem>().enabled)
            playerName.GetComponent<Text>().text = aiNames[Random.Range(0, aiNames.Length)];
        else
        playerName.GetComponent<Text>().text = PlayerPrefs.GetString("PlayerName");

        playerName.GetComponent<NameUIController>().carRend = carMesh;

    }

    public void CalcEngineSound()
    {
        float gearPercentage = (1 / (float)numGears);
        float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1),
                                                    Mathf.Abs(currentSpeed / maxSpeed));
        currentGearPerc = Mathf.Lerp(currentGearPerc, targetGearFactor, Time.deltaTime * 5f);

        var gearNumFactor = currentGear / (float)numGears;
        rpm = Mathf.Lerp(gearNumFactor, 1, currentGearPerc);

        float speedPercentage = Mathf.Abs(currentSpeed / maxSpeed);
        float upperGearMax = (1 / (float)numGears) * (currentGear + 1);
        float downGearMax = (1 / (float)numGears) * currentGear;

        if (currentGear > 0 && speedPercentage < downGearMax)
            currentGear--;

        if(speedPercentage > upperGearMax && (currentGear < (numGears-1)))
                currentGear++;

        float pitch = Mathf.Lerp(lowPitch, highPitch, rpm);
        highAcc.pitch = Mathf.Min(highPitch, pitch) * 0.25f;
    }

    public void Go(float acc, float steer, float brake)
    {
        acc = Mathf.Clamp(acc, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        brake = Mathf.Clamp(brake, 0, 1) * maxBrakeTorque;

        if(brake != 0)
        {
            brakeLight.SetActive(true);
        }
        else
        {
            brakeLight.SetActive(false);
        }

        float thrustTorque = 0;
        if(currentSpeed < maxSpeed)
            thrustTorque = acc * torque;

        for (int i = 0; i < 4; i++)
        {


            WCs[i].motorTorque = thrustTorque;

            if (i < 2)
            {
                WCs[i].steerAngle = steer;
            }
            else
                WCs[i].brakeTorque = brake;

            Quaternion quat;
            Vector3 position;
            WCs[i].GetWorldPose(out position, out quat);
            Wheels[i].transform.position = position;
            Wheels[i].transform.rotation = quat;
        }
    }

    public void CheckForSkid()
    {
        int numSkidding = 0;
        for(int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            WCs[i].GetGroundHit(out wheelHit);

            if(Mathf.Abs(wheelHit.forwardSlip) >= 0.4f || Mathf.Abs(wheelHit.sidewaysSlip) >= 0.4f)
            {
                numSkidding++;
                if(!skidSound.isPlaying)
                {
                    skidSound.Play();
                }
                //StartSkidTrail(i);
                skidSmoke[i].transform.position = WCs[i].transform.position - WCs[i].transform.up * WCs[i].radius;
                skidSmoke[i].Emit(1);
            }
            else
            {
               //EndSkidTrail(i);
            }
        }

        if(numSkidding == 0 && skidSound.isPlaying)
        {
            skidSound.Stop();
        }
    }

    private IEnumerator timedLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(.7f);
            radius = 6 + KPH / 20;

        }
    }
    private bool checkGears()
    {
        if (KPH >= currentGear) return true;
        else return false;
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
}
