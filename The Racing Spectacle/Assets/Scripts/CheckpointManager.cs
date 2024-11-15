using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public int lap = 0;
    public int checkPoint = -1;
    public float timeEntered = 0;
    int checkPointCount;
    int nextCheckPoint;
    public GameObject lastCP;
    public RaceMonitor race;
    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        race = GameObject.FindWithTag("Monitor").GetComponent<RaceMonitor>();
        GameObject[] cps = GameObject.FindGameObjectsWithTag("checkpoint");
        checkPointCount = cps.Length;
        foreach(GameObject c in cps)
        {
            if(c.name == "0")
            {
                lastCP = c;
                break;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "checkpoint")
        {
            int thisCPNumber = int.Parse(other.gameObject.name);
            if(thisCPNumber == nextCheckPoint)
            {
                lastCP = other.gameObject;
                checkPoint = thisCPNumber;
                timeEntered = Time.time;
                if (checkPoint == 0) lap++;

                nextCheckPoint++;
                if(nextCheckPoint >= checkPointCount)
                {
                    nextCheckPoint = 0;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if(gameObject.CompareTag("Car"))
        {
            if(checkPoint == 4)
            {
                StartCoroutine(Waiting());
               if(i == 1)
                {
                    race.gameOverPanel.SetActive(true);
                    
                }
            }
        }
    }

    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(5);
        i = 1;
    }
}
