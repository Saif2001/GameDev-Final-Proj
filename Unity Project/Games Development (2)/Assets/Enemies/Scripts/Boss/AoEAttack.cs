using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEAttack : MonoBehaviour
{

    public bool PlayerinKillzone;

    //VoidTriggerEnter used to check if player should take damage
     void OnTriggerEnter(Collider other)
     {
        PlayerinKillzone = true;
     }

    //On exit, player is safe from damage
    void OnTriggerExit(Collider other) {
        PlayerinKillzone = false;
    }

    //Bool value passed to another script.

}
