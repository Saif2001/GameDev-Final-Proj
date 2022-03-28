using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullets : MonoBehaviour
{

    //Function to decrement player health when hit by boss bullets
    public Transform Player;
    public int bulletDamage = 20;

    // Bullet prefab saved as a trigger.
    private void OnTriggerEnter(Collider other)
    {
        //If player is hit, decrement health
        if(other.CompareTag("Player") == true)
        {
            Debug.Log("Bullet hit player");
            GameObject.Find("Player_Scene1").GetComponent<PlayerController>().health -= bulletDamage;
        }
    }
}
