using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  This script controls the projectile spawned when a flying enemy attacks
 */

public class FlyingAttack : MonoBehaviour
{
    EnemyParameters parameters;

    private float windupTimer;
    private float maximumWindupTime;
    private float damage;

    public float speed = 20f;

    // Initialise some parameters
    public void Initialise(float WindupTime, EnemyParameters Parameters)
    {
        parameters = Parameters;
        transform.LookAt(parameters.player.position);

        windupTimer = WindupTime;
        maximumWindupTime = parameters.maximumFlyingAttackWindupTimer;
        damage = parameters.attackDamageFlying;
    }
    
    // Each frame, move the projectile forwards according to some speed set in the inspector
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    // On collision, apply damage if the player was hit and destroy the projectile regardless of the collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(parameters.attackDamageRanged);
        }
        Destroy(this.gameObject);
    }
}
