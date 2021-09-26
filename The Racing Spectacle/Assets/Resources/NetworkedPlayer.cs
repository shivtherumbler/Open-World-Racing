using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun;

public class NetworkedPlayer : MonoBehaviourPunCallbacks
{
    public static GameObject LocalPlayerInstance;
    public GameObject playerNamePrefab;
    public Rigidbody rb;
    public Renderer carMesh;

    private void Start()
    {
        if(photonView.IsMine)
        {
            //LocalPlayerInstance = gameObject;
            GameObject playerName = Instantiate(playerNamePrefab);
            playerName.GetComponent<NameUIController>().target = rb.gameObject.transform;
            playerName.GetComponent<Text>().text = photonView.Owner.NickName;
            playerName.GetComponent<NameUIController>().carRend = carMesh;
        }
        else
        {
            GameObject playerName = Instantiate(playerNamePrefab);
            playerName.GetComponent<NameUIController>().target = rb.gameObject.transform;
            playerName.GetComponent<Text>().text = photonView.Owner.NickName;
            playerName.GetComponent<NameUIController>().carRend = carMesh;
        }
    }
}
