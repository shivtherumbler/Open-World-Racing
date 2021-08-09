using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circuit : MonoBehaviour
{

    public List<Transform> waypoints = new List<Transform>();

    private void OnDrawGizmos()
    {
        DrawGizmos(false);
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmos(true);
    }

    private void DrawGizmos(bool selected)
    {
        if (selected == false) return;

        Transform[] path = GetComponentsInChildren<Transform>();

        waypoints = new List<Transform>();
        for (int i = 1; i < path.Length; i++)
        {
            waypoints.Add(path[i]);
        }
        if (waypoints.Count > 1)
        {
            Vector3 prev = waypoints[0].transform.position;
            for(int i =1; i < waypoints.Count; i++)
            {
                Vector3 next = waypoints[i].transform.position;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
            Gizmos.DrawLine(prev, waypoints[0].transform.position);
        }
    }
}
