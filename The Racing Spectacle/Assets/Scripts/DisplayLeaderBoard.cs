using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLeaderBoard : MonoBehaviour
{
    public Text first;
    public Text second;
    public Text third;
    public Text fourth;
    public Text fifth;
    public Text sixth;

    void Start()
    {
        LeaderBoard.Reset();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        List<string> places = LeaderBoard.GetPlaces();
        if(places.Count > 0)
            first.text = places[0];
        if (places.Count > 1)
            second.text = places[1];
        if (places.Count > 2)
            third.text = places[2];
        if (places.Count > 3)
            fourth.text = places[3];
        if (places.Count > 4)
            fifth.text = places[4];
        if (places.Count > 5)
            sixth.text = places[5];
    }
}
