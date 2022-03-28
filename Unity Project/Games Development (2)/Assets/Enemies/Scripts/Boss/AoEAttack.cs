using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEAttack : MonoBehaviour
{

    public bool PlayerinKillzone;

    // Start is called before the first frame update
     void OnTriggerEnter(Collider other)
     {
        PlayerinKillzone = true;
     }

    void OnTriggerExit(Collider other) {
        PlayerinKillzone = false;
    }

}
