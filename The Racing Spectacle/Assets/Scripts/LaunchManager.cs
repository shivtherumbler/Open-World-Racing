using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Photon.Pun;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    byte maxPlayersPerRoom = 6;
    bool isConnecting;
    public InputField playerName;
    public Text feedbackText;
    string gameVersion = "1";
    public GameObject multiplay;
    public GameObject singleplay;
    public GameObject join;
    public GameObject host;
    public GameObject backButton;

    public bool TriesToConnectToMaster;
    public bool TriesToConnectToRoom;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PlayerPrefs.HasKey("PlayerName"))
            playerName.text = PlayerPrefs.GetString("PlayerName");
    }

    void Start()
    {
        TriesToConnectToMaster = false;
        TriesToConnectToRoom = false;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Multiplayer()
    {
        multiplay.SetActive(true);
        singleplay.SetActive(false);
        backButton.SetActive(true);
    }

    public void MultiplayBack()
    {
        multiplay.SetActive(false);
        singleplay.SetActive(true);
        join.SetActive(false);
        host.SetActive(false);
        backButton.SetActive(false);
    }

    public void OnJoinButtonClick()
    {
        multiplay.SetActive(false);
        join.SetActive(true);
    }
    public void OnHostButtonClick()
    {
        multiplay.SetActive(false);
        host.SetActive(true);
    }

    public void ConnectNetwork()
    {
        RaceMonitor.totalLaps = 1;
        feedbackText.text = "";
        isConnecting = true;

        PhotonNetwork.NickName = playerName.text;
        if(PhotonNetwork.IsConnected)
        {
            feedbackText.text += "\nJoining Room... ";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            feedbackText.text += "\nConnecting... ";
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnHostRoom(Text RoomName)
    {
        RaceMonitor.totalLaps = 1;
        feedbackText.text = "";
        isConnecting = true;

        PhotonNetwork.NickName = playerName.text;
        if (!PhotonNetwork.IsConnected)
            return;
        TriesToConnectToRoom = true;  
        PhotonNetwork.CreateRoom(RoomName.text, new RoomOptions { MaxPlayers = 6 });                 
    }
    public void OnJoinRoom(Text RoomName)
    {
        RaceMonitor.totalLaps = 1;
        feedbackText.text = "";
        isConnecting = true;

        PhotonNetwork.NickName = playerName.text;
        if (!PhotonNetwork.IsConnected)
            return;
        TriesToConnectToRoom = true;
        PhotonNetwork.JoinRoom(RoomName.text);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(message);
        base.OnCreateRoomFailed(returnCode, message);
        TriesToConnectToRoom = false;
        SceneManager.LoadScene("Matchmaking");
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        TriesToConnectToRoom = false;
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("Multi Mode");
    }
    public override void OnJoinedRoom()
    {
        
        base.OnJoinedRoom();
        TriesToConnectToRoom = false;
        PhotonNetwork.AutomaticallySyncScene = true;
        //Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name);
    }

    public void SetName(string name)
    {
        name = playerName.text;
        PlayerPrefs.SetString("PlayerName", name);
        Debug.Log(name);
    }

    public void SetLaps1()
    {
        RaceMonitor.totalLaps = 1;
    }

    public void SetLaps3()
    {
        RaceMonitor.totalLaps = 3;
    }
    public void SetLaps5()
    {
        RaceMonitor.totalLaps = 5;
    }

    public void SetLaps7()
    {
        RaceMonitor.totalLaps = 7;
    }

    public void Connect()
    {
        SceneManager.LoadScene("Select Mode");

    }

    public void Morning()
    {
        SceneManager.LoadScene("Day Mode");
        
    }

    public void Sunset()
    {
        SceneManager.LoadScene("Sunset Mode");
        
    }

    public void Night()
    {
        SceneManager.LoadScene("Night Mode");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("AwakeScene");
    }

    public void HowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }
    public void Quit()
    {
        Application.Quit();
    }

    // Network Callbacks

    public override void OnConnectedToMaster()
    {
        if(isConnecting)
        {
            feedbackText.text += "\nOnConnectedToMaster... ";
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        feedbackText.text += "\nFailed to join random room. ";
        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = this.maxPlayersPerRoom });
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        feedbackText.text += "\nDisconnected because " + cause;
        isConnecting = false;
    }

    /*public override void OnJoinedRoom()
    {
        feedbackText.text += "\nJoined room with " + PhotonNetwork.CurrentRoom.PlayerCount + " players.";
        PhotonNetwork.LoadLevel("Multi Mode");
    }*/
}
