using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{

    public bool readyToTransition = false;      //Fires when conditions are correct to continue

    private void Update()
    {
        GameObject Player = GameObject.Find("Player");  
    }
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("LEVEL FINISHED");
        readyToTransition = true;
            
    }
}
