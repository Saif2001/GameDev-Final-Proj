using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullets : MonoBehaviour
{

    public Transform Player;
    public int bulletDamage = 20;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            Debug.Log("Bullet hit player");
            GameObject.Find("Player_Scene1").GetComponent<PlayerController>().health -= bulletDamage;
        }
    }
}
