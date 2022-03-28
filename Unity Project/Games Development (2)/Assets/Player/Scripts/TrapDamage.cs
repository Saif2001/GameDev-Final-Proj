using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Checks if player is colliding with tag
        if(other.gameObject.CompareTag("Trap Injury") ) 
        {
            //player losing 50 health
            GameObject.Find("Player_Scene1").GetComponent<PlayerController>().health = 50;
        }

        if(other.gameObject.CompareTag("InstaKill") ) 
        {   
            //kills the player
            GameObject.Find("Player_Scene1").GetComponent<PlayerController>().health = 00;
        }

    }
}
