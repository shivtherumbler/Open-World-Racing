using UnityEngine;
using UnityEngine.UI;


using Photon.Realtime;
using Photon.Pun;
public class MultiManager : MonoBehaviourPunCallbacks
{
    public VehicleList list;
    public GameObject[] carPrefabs;
    public controller control;
    public InputManager input;
    public GameObject needle;
    //public Transform[] startPos;
    private float startPosition = 18f, endPosition = -198f;
    private float desiredPosition;
    public Text kph;
    public Text gearNum;
    public Slider nitrusSlider;


    public float vehicleSpeed;

    // Start is called before the first frame update
    private void Start()
    {
        //startPos = GameObject.Find("Start Positions").GetComponentsInChildren<Transform>();
        //int randomStartPos = Random.Range(0, startPos.Length);


        //foreach (Transform t in startPos)
        //{
            //if (t == startPos[randomStartPos]) continue;
            //GameObject car = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)]);
            //car.transform.position = t.position;
            //car.transform.rotation = t.rotation;
        //}

        /*for(int i=0;i<list.vehicles.Length;i++)
        {
            Instantiate(list.vehicles[PlayerPrefs.GetInt("pointer")], startPos[i].transform.position, startPos[i].transform.rotation);
        }*/
        
        //list.vehicles[PlayerPrefs.GetInt("pointer")].tag = "Player";
        //control = GameObject.FindGameObjectWithTag("Player").GetComponent<controller>();
        //input = GameObject.FindGameObjectWithTag("Player").GetComponent<InputManager>();
    }
    private void Update()
    {
        Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name);
    }

    public void OnJoinRoom(Text RoomName)
    {
        //int i= 0;
        //Instantiate(list.vehicles[PlayerPrefs.GetInt("pointer")], startPos[i].transform.position, startPos[i].transform.rotation);


    }
    
    private void FixedUpdate()
    {
        vehicleSpeed = control.kph;
        kph.text = control.kph.ToString("0");
        updateNeedle();
        changeGear();
        nitrusUI();
    }


    public void updateNeedle()
    {
        desiredPosition = startPosition - endPosition;
        float temp = vehicleSpeed / 180;
        if(control.kph < 30)
        {
            needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition));
        }
        else if (control.kph >= 30 && control.kph < 60)
        {
            needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition) + 20);
        }
        else if (control.kph >= 60 && control.kph < 90)
        {
            needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition) + 40);
        }
        else if (control.kph >= 90 && control.kph < 120)
        {
            needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition) + 60);
        }
        else if (control.kph >= 120 && control.kph < 150)
        {
            needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition) + 80);
        }
        else if (control.kph >= 150 && control.kph < 180)
        {
            needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition) + 100);
        }
        else if(control.kph >= 180 && control.kph < 210)
        {
            needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition) + 120);
        }
        else if (control.kph >= 210 && control.kph < 250)
        {
            needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition) + 150);
        }
        else if (control.kph >= 250)
        {
            needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - temp * desiredPosition) + 200);
        }

    }

    public void changeGear()
    {

        gearNum.text = (control.currentGear + 1).ToString();

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D) == false)
        {
            gearNum.text = ("R").ToString();
        }

        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.D) == false && Input.GetKey(KeyCode.S) == false)
        {
            gearNum.text = ("B").ToString();
        }

        if (vehicleSpeed < 1 && vehicleSpeed > -1)
        {
            gearNum.text = ("N").ToString();
        }

    }

    public void nitrusUI()
    {
        nitrusSlider.value = control.nitrusValue;
    }

    public void AccelerateDown()
    {
        input.vertical = 1;
    }
    
    public void AccelerateUp()
    {
        input.vertical = 0;
    }

    public void BrakeDown()
    {
        input.vertical = -1;
    }

    public void BrakeUp()
    {
        input.vertical = 0;
    }

}
