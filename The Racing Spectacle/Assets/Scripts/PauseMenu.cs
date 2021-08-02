using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public controller control;
    public static bool GameIsPaused = false;
    public static bool racing = false;
    public GameObject PausePanel;

    // Start is called before the first frame update
    void Start()
    {
        PausePanel.SetActive(false);
        control = GameObject.FindGameObjectWithTag("Player").GetComponent<controller>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
                
            }
            else
            {
                Pause();
               
            }
        }
        
    }

    public void Resume()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        
    }

    public void Steering()
    {
        control.steering = true;
        control.joystick = false;
        control.gyroscope = false;
    }

    public void JoyStick()
    {
        control.joystick = true;
        control.steering = false;
        control.gyroscope = false;
    }

    public void Automatic()
    {
        control.gyroscope = true;
        control.steering = false;
        control.joystick = false;
    }

}
