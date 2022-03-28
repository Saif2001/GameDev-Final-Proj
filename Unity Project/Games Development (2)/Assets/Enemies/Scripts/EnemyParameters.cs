using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 *  This script is a list of common parameters used by enemies. By keeping the list in a single script on the same object as the enemy manager, 
 *  these variables can be changed without needing to go through and update every single enemy
 */

// An enum containing all possible enemy states
public enum EnemyState
{
    idling,
    chasing,
    hunting,
    attacking
}

// A script full of variable declarations and helper functions common to all enemy types
public class EnemyParameters : MonoBehaviour
{
    // A reference to the player's transform
    public Transform player;

    // Detection parameters
    [Header("Detection Parameters")]
    public float fieldOfView = 270f;
    public float perceptionRange = 10f;
    public float separationRange = 5f;
    public Vector3[] avoidanceRayDirections;
    public LayerMask playerLayerMask;
    public LayerMask obstacleLayerMask;
    [HideInInspector] public LayerMask combinedLayerMask;

    // Movement parameters
    [Header("Movement parameters")]
    public float maximumSpeed;
    public float minimumTurningTime;
    public float flyingSpeedPenalty = 0.5f;

    // Idling parameters
    [Header("Idling Parameters")]
    [Range(0, 1)] public float idlingMovementPenalty = 0.7f;
    public float maximumIdlingTimer = 2f;
    public float idlingRadius;
    public Transform flyingIdlePoint;

    // Chasing Parameters

    // Hunting Parameters
    [Header("Hunting Parameters")]
    [Range(0, 1)] public float huntingMovementPenalty = 0.9f;
    public float huntingRadius = 5f;
    public float maximumHuntingTimer = 8f;

    // Attacking Parameters
    [Header("Attacking Parameters")]
    [Range(0, 1)] public float attackingMovementPenalty = 0.4f;
    public float attackDamageMelee = 10f;
    public float attackRangeMelee = 2f;
    public float maximumAttackTimerMelee = 3f;
    public float maximumMeleeAttackWindupTimer = 1.5f;
    public float maximumMeleeAttackWinddownTimer = 1.5f;
    public float attackDamageRanged = 5f;
    public float attackRangeRanged = 10f;
    public float maximumAttackTimerRanged = 3f;
    public float maximumRangedAttackWindupTimer = 1.5f;
    public float maximumRangedAttackWinddownTimer = 1.5f;
    public float attackDamageFlying = 5f;
    public float attackRangeFlying = 10f;
    public float maximumAttackTimerFlying = 3f;
    public float maximumFlyingAttackWindupTimer = 1.5f;
    public float maximumFlyingAttackWinddownTimer = 1.5f;
    public float attackForceFlying = 5f;

    // Hovering Parameters
    [Header("Hovering Parameters")]
    public float upperHoveringBound;
    public float lowerHoveringBound;

    // Steering weights
    [Header("Steering Weights")]
    public float separationWeight;
    public float cohesionWeight;
    public float alignmentWeight;
    public float avoidanceWeight;
    public float seekingWeight;
    public float boundsWeight;
    public float hoveringWeight;

    private void Start()
    {
        combinedLayerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Obstacles"));
    }


    // A function to determine whether the player is in line of sight of an enemy
    public bool DetectPlayer(Transform enemy)
    {
        // Get the normalised direction vector pointing from the enemy to the player
        Vector3 playerDirection = (player.position - enemy.position).normalized;
        // Check whether the player is currently in the field of view
        if (IsInFOV(enemy, player))
        {
            // Use a raycast to determine whether the player is in detection range and not behind an obstacle
            RaycastHit hit;
            if (Physics.Raycast(enemy.position + (playerDirection * 0.1f), playerDirection, out hit, perceptionRange))
            {
                // If the ray connected with the player then they are within line of sight
                if (hit.collider.tag == "Player")
                    return true;
            }
        }
        return false;
    }


    // Check whether the target is within the field of view an actor
    public bool IsInFOV(Transform actor, Transform target)
    {
        // Get the angle between the target and the forward transform. Unsigned angle is used to give a value between 0 and 180 degrees
        float angle = Vector3.Angle(actor.forward, (target.position - actor.position));
        // Return true if the angle is within the field of view
        return (angle <= fieldOfView / 2);
    }

    // Check whether the player is within a certain range of an enemy
    public bool IsPlayerInRange(float range, Transform enemy)
    {
        if (Vector3.SqrMagnitude(enemy.position - player.position) <= (range * range))
        {
            return true;
        }
        return false;
    }
}
