using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class NetworkController : MonoBehaviourPunCallbacks
{

    public bool TriesToConnectToMaster;
    public bool TriesToConnectToRoom;

    void Start()
    {
        TriesToConnectToMaster = false;
        TriesToConnectToRoom = false;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {

    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        TriesToConnectToMaster = false;
        Debug.Log("Connected to Master!");
    }


    public void OnHostRoom(Text RoomName)
    {
        if (!PhotonNetwork.IsConnected)
            return;
        TriesToConnectToRoom = true;  
        PhotonNetwork.CreateRoom(RoomName.text, new RoomOptions { MaxPlayers = 2 });                 
    }
    public void OnJoinRoom(Text RoomName)
    {
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
        PhotonNetwork.LoadLevel("TicTacToe");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        TriesToConnectToRoom = false;
        PhotonNetwork.AutomaticallySyncScene = true;
//        Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name);
    }

}
