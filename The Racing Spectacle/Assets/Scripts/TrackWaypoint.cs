using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackWaypoint : MonoBehaviour
{
    public Color lineColor;
    [Range(0, 1)] public float sphereRadius;
    public List<Transform> waypoint = new List<Transform>();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = lineColor;

        Transform[] path = GetComponentsInChildren<Transform>();

        waypoint = new List<Transform>();
        for(int i = 1; i< path.Length; i++)
        {
            waypoint.Add(path[i]);
        }

        for(int i = 0; i<waypoint.Count;i++)
        {
            Vector3 currentWaypoint = waypoint[i].position;
            Vector3 prevWaypoint = Vector3.zero;

            if (i != 0)
            {
                prevWaypoint = waypoint[i - 1].position;
            }
            else if(i == 0)
            {
                prevWaypoint = waypoint[waypoint.Count - 1].position;
            }

            Gizmos.DrawLine(prevWaypoint, currentWaypoint);
            Gizmos.DrawSphere(currentWaypoint, sphereRadius);
        }
     }
}

