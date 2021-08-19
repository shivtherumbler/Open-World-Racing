using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Photon.Pun;

public class MultiMonitor : MonoBehaviourPunCallbacks
{
    public GameObject[] countDownItems;
    CheckpointManager[] carsCPM;
    public VehicleList list;

    public GameObject[] carPrefabs;
    public Transform[] spawnPos;

    public static bool racing = false;
    public static int totalLaps = 1;
    public GameObject gameOverPanel;
    //public GameObject HUD;

    public GameObject startRace;
    public GameObject WaitingText;

    int playerCar;

   
    // Start is called before the first frame update
    void Start()
    {
        racing = false;

        foreach (GameObject g in countDownItems)
            g.SetActive(false);

        gameOverPanel.SetActive(false);

        if (Application.loadedLevelName == "Multi Mode")
        {
            list = null;
        }

        if (Application.loadedLevelName == "Multi Mode")
        {
            if (PhotonNetwork.IsMasterClient)
                startRace.SetActive(true);
            else
                WaitingText.SetActive(true);
        }
        else
        {
            startRace.SetActive(false);
            WaitingText.SetActive(false);
        }

        playerCar = PlayerPrefs.GetInt("PlayerCar");
        int randomStartPos = Random.Range(0, spawnPos.Length);
        Vector3 startPos = spawnPos[randomStartPos].position;
        Quaternion startRot = spawnPos[randomStartPos].rotation;
        GameObject pCar = null;

        if (PhotonNetwork.IsConnected)
        {
            startPos = spawnPos[PhotonNetwork.LocalPlayer.ActorNumber - 1].position;
            startRot = spawnPos[PhotonNetwork.LocalPlayer.ActorNumber - 1].rotation;

            if (NetworkedPlayer.LocalPlayerInstance == null)
            {
                pCar = PhotonNetwork.Instantiate(carPrefabs[playerCar].name, startPos, startRot, 0);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                startRace.SetActive(true);
            }
            else
            {
                WaitingText.SetActive(true);
            }
        }

        {
            pCar = list.vehicles[PlayerPrefs.GetInt("pointer")];
            //pCar.tag = "Player";
            pCar.transform.position = startPos;
            pCar.transform.rotation = startRot;

            foreach (Transform t in spawnPos)
            {
                if (t == spawnPos[randomStartPos]) continue;
                //if(Application.loadedLevelName == "SampleScene")
                //{
                GameObject car = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)]);
                car.transform.position = t.position;
                car.transform.rotation = t.rotation;
                //}

            }

            StartGame();
        }


        //SmoothFollow.playerCar = pCar.gameObject.GetComponent<Drive>().rb.transform;
        //pCar = GameObject.FindGameObjectWithTag("Player");
        //pCar.GetComponent<controller>().enabled = true;


    }

    public void BeginGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartGame", RpcTarget.All, null);
        }
    }

    [PunRPC]
    public void StartGame()
    {
        StartCoroutine(PlayCountDown());
        startRace.SetActive(false);
        WaitingText.SetActive(false);

        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        carsCPM = new CheckpointManager[cars.Length];
        for (int i = 0; i < cars.Length; i++)
        {
            carsCPM[i] = cars[i].GetComponent<CheckpointManager>();
        }
    }

    IEnumerator PlayCountDown()
    {
        yield return new WaitForSeconds(2);
        foreach (GameObject g in countDownItems)
        {
            g.SetActive(true);
            yield return new WaitForSeconds(1);
            g.SetActive(false);
        }
        racing = true;
    }

    [PunRPC]
    public void RestartGame()
    {
        PhotonNetwork.LoadLevel("Multi Mode");
    }

    public void RestartLevel()
    {
        racing = false;
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("RestartGame", RpcTarget.All, null);
        }
        else
        {

        }
        if (SceneManager.GetActiveScene().name == "Day Mode")
        {
            SceneManager.LoadScene("Day Mode");
        }
        if (SceneManager.GetActiveScene().name == "Night Mode")
        {
            SceneManager.LoadScene("Night Mode");
        }
        if (SceneManager.GetActiveScene().name == "Sunset Mode")
        {
            SceneManager.LoadScene("Sunset Mode");
        }
    }

    public void DisconnectPlayer()
    {
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        SceneManager.LoadScene("AwakeScene");
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;

    }
    //[PunRPC]
    //public override void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //PhotonNetwork.CurrentRoom.IsOpen = false;

    //}
    [PunRPC]
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.DestroyAll();
        StartCoroutine(DisconnectAndLoad());
    }

    public void MainMenu()
    {
        racing = false;
        SceneManager.LoadScene("AwakeScene");

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!racing) return;
        if (racing == true)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        int finishedCount = 0;
        foreach (CheckpointManager cpm in carsCPM)
        {
            if (cpm.lap == totalLaps + 1)
            {
                finishedCount++;
            }
            if (finishedCount == carsCPM.Length)
            {
                //HUD.SetActive(false);
                gameOverPanel.SetActive(true);
            }

        }

    }

}
