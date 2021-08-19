using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject Player;
    private controller control;
    private GameObject child;
    private GameObject cameraLookat;
    public float speed;
    public float defaultFOV = 0, desiredFOV = 0;
    [Range(0,5)]public float smoothTime = 0;

    
    private void Start()
    {
        if(Application.loadedLevelName != "Multi Mode")
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        
        child = Player.transform.Find("camera constraint").gameObject;
        cameraLookat = Player.transform.Find("camera lookAt").gameObject;
        control = Player.GetComponent<controller>();
        defaultFOV = UnityEngine.Camera.main.fieldOfView;
    }

    private void FixedUpdate()
    {
        follow();
        boostFOV();

        //speed = (control.kph >= 80) ? 20: control.kph / 4;
    }

    private void follow()
    {
        speed = Mathf.Lerp(speed, control.kph / 2, Time.deltaTime);

        gameObject.transform.position = Vector3.Lerp(transform.position, child.transform.position, Time.deltaTime * speed);
        gameObject.transform.LookAt(Player.gameObject.transform.position);
    }

    private void boostFOV()
    {
        if(control.nitrusFlag)
        {
            UnityEngine.Camera.main.fieldOfView = Mathf.Lerp(UnityEngine.Camera.main.fieldOfView, desiredFOV, Time.deltaTime * smoothTime);
        }
        else
        {
            UnityEngine.Camera.main.fieldOfView = Mathf.Lerp(UnityEngine.Camera.main.fieldOfView, defaultFOV, Time.deltaTime * smoothTime);

        }
    }

}
