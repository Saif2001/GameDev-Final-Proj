using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] GameObject[] waypoints;
    [SerializeField] float speed = 1f;
    int currentWaypointIndex = 0;
    
    void Update()
    {
        //vector3 defines the position of the object
        //move.towards calculates a new position between two objects
        if(Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < .1f)
        {
            currentWaypointIndex++;
            if(currentWaypointIndex >= waypoints.Length){
                currentWaypointIndex = 0;
            }

        }
        transform.position = Vector3.MoveTowards(transform.position,waypoints[currentWaypointIndex].transform.position,speed *Time.deltaTime);
    }
}
