using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{

   // [SerializeField] public GameObject[] Checkpoints;
  
    public Vector3 spawnPoint;
    public Vector3 StartingSpawn;
    //current checkpoint index
    //int CurrentCPIndex = 0;

    void Start()
    {
        spawnPoint = gameObject.transform.position;
        StartingSpawn = gameObject.transform.position;
        Debug.Log("Starting spawn");
        
    }


/*  void Update()
    {
        
        //temp death barrier connected to y-axis
        if (gameObject.transform.position.y < 135f)
        {
            gameObject.transform.position = spawnPoint;
        }
        
            }
*/
    //checkpoint changer
    private void OnTriggerEnter(Collider other) 
    {   
        if(other.gameObject.CompareTag("Checkpoint"))
        {
        spawnPoint = other.transform.position;
        Debug.Log("SpawnPoint Set");
        }




        /*if(other.gameObject.CompareTag("Checkpoint"))
        {   
            spawnPoint = Checkpoints[CurrentCPIndex].transform.position;
            Destroy(Checkpoints[CurrentCPIndex]);
            CurrentCPIndex++;
            Debug.Log("SpawnPoint Set");
        }
        */

    }
        public void RestartLastCP()
    {
        GameObject.Find("Player_Scene1").transform.position = GameObject.Find("Player_Scene1").GetComponent<CheckpointSystem>().spawnPoint;
        GameObject.Find("Player_Scene1").GetComponent<PlayerController>().health = 100;
    }



}
