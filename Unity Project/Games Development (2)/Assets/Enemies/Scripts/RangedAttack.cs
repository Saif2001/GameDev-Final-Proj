using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  This script controls the projectile spawned by ranged enemies when they attack
 */

public class RangedAttack : MonoBehaviour
{
    private EnemyParameters parameters;
    public float speed = 20f;

    // Initialse various parameters
    public void Initialise(EnemyParameters Parameters)
    {
        parameters = Parameters;
        transform.LookAt(parameters.player.position);
    }

    // Once per frame, move the projectile forwards according to some set speed
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    // If the projectile collides with something, destroy the projectile. If the collision was with the player, deal some damage
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(parameters.attackDamageRanged);
        }
        Destroy(this.gameObject);
    }
}
