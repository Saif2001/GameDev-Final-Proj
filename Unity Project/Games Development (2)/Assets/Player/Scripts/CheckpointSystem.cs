using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{

    //variable for storing a spawn location
    public Vector3 spawnPoint;
    //variable for storing the starting spawn location
    public Vector3 StartingSpawn;


    void Start()
    { 
        //both variables are given the players position vector
        spawnPoint = gameObject.transform.position;
        StartingSpawn = gameObject.transform.position;
    }



    //function for colliding with any checkpoint object
    private void OnTriggerEnter(Collider other) 
    {   //if the object is tagged a checkpoint
        if(other.gameObject.CompareTag("Checkpoint"))
        {
        //set the spawn location by using the position of the object
        spawnPoint = other.transform.position;
        }

    }
    
        public void RestartLastCP()
    {   
        //variable saetup for UI restart from last checkpoint button in the pause Ui
        GameObject.Find("Player_Scene1").transform.position = GameObject.Find("Player_Scene1").GetComponent<CheckpointSystem>().spawnPoint;
        //gives the players full health and 5 charges in the gunsabre
        GameObject.Find("Player_Scene1").GetComponent<PlayerController>().health = 100;
        GameObject.Find("Player_Scene1").GetComponent<PlayerController>().chargeBar = 5;
        
    }



}
