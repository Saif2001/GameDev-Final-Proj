using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  A script to damage the player/enemies when hit by the saw blade in the combat arena
 */

public class MovingSaw : MonoBehaviour
{
    public float damage;

    // If the blade collider is trigger, apply damage if the collider was the player or an enemy
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }

        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyController>().GetHit();
        }
    }
}
