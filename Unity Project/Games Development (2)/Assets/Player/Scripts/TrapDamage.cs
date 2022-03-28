using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.CompareTag("Trap Injury") ) 
        {
            GameObject.Find("Player_Scene1").GetComponent<PlayerController>().health = 50;
            Debug.Log("Injury working");
        }

        if(other.gameObject.CompareTag("InstaKill") ) 
        {
            GameObject.Find("Player_Scene1").GetComponent<PlayerController>().health = 00;
            Debug.Log("InstaKill working");
        }

    }
}
