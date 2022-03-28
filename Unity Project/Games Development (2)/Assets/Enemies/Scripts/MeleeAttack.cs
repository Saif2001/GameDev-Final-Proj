using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  A script to handle melee enemy attacks
 */

public class MeleeAttack : MonoBehaviour
{
    private EnemyParameters parameters;

    public void Initialise(EnemyParameters Parameters)
    {
        parameters = Parameters;
    }

    // If the enemy blade collider strikes the player, deal damage to them
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(parameters.attackDamageMelee);
        }
    }
}
