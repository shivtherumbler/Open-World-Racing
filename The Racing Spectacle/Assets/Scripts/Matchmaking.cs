using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Matchmaking : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject JoinField;
    public GameObject HostField;
    public GameObject Join;
    public GameObject Host;
    public void OnJoinButtonClick()
    {
        Join.SetActive(false);
        Host.SetActive(false);
        JoinField.SetActive(true);

    }
    public void OnHostButtonClick()
    {
        Join.SetActive(false);
        Host.SetActive(false);
        HostField.SetActive(true);
    }
    public void OnBackButtonClick()
    {
        SceneManager.LoadScene("Menu");
    }
}
