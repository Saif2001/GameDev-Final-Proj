using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 *  A script to control melee enemies. Inherits from the generic EnemyController class
 */

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class MeleeEnemyController : EnemyController
{
    public Animator enemyAnimator;

    private Rigidbody enemyRigidbody;
    private NavMeshAgent enemyNavMeshAgent;
    private Manager manager;
    private EnemyParameters parameters;
    private Transform enemy;
    private MeleeAttack bladeController;

    private EnemyState currentState = EnemyState.idling;
    private EnemyState nextState = EnemyState.idling;

    private bool isPlayerDetected = false;

    private Vector3 idlePoint;
    private float idlingTimer;

    private Vector3 lastKnownPosition;
    private float huntingTimer;

    private bool isAttacking = false;
    private float attackTimer;
    private float attackWindupTimer;
    private float attackWinddownTimer;

    // Returns the type of the enemy (used by flying enemies to determine whether their neighbours are flying or grounded)
    public override string Type()
    {
        return "Melee";
    }

    // A function that is called any time this enemy is hit by the player
    public override void GetHit()
    {
        Debug.Log("Ouch!");
        this.gameObject.SetActive(false);
        // Remove this enemy from its corresponding enemy manager
        manager.enemies.Remove(this);
    }

    // Initialise various parameters
    public override void Initialise(Manager _manager, EnemyParameters Parameters, Vector3 SpawnPosition)
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        parameters = Parameters;
        enemy = this.transform;
        manager = _manager;
        idlePoint = SpawnPosition;
        idlingTimer = parameters.maximumIdlingTimer;
        huntingTimer = parameters.maximumHuntingTimer;
        attackTimer = 0;
        attackWindupTimer = parameters.maximumMeleeAttackWindupTimer;
        attackWinddownTimer = parameters.maximumMeleeAttackWinddownTimer;
        enemyNavMeshAgent.stoppingDistance = parameters.attackRangeMelee * 0.9f;
        bladeController = GetComponentInChildren<MeleeAttack>();
        bladeController.Initialise(parameters);
    }

    // Called once per frame by the enemy manager to handle movement/FSM functionality
    public override void Move(List<EnemyController> Neighbours)
    {
        // Check whether the player is in line of sight of this enemy
        isPlayerDetected = parameters.DetectPlayer(enemy);
        // Update the enemy's finite state machine
        UpdateStates();
        CountDown();
        // Set the speed float of the animator
        float velocity = enemyNavMeshAgent.velocity.magnitude / enemyNavMeshAgent.speed;
        enemyAnimator.SetFloat("speed", velocity);
    }

    // Update the enemy FSM
    private void UpdateStates()
    {
        switch (currentState)
        {
            case EnemyState.idling:
                Idle();
                IdlingTransitions();
                break;
            case EnemyState.chasing:
                Chase();
                ChasingTransitions();
                break;
            case EnemyState.hunting:
                Hunt();
                HuntingTransitions();
                break;
            case EnemyState.attacking:
                Attack();
                AttackingTransitions();
                break;
            default:
                Debug.Log("Error in MeleeEnemyController.cs: UpdateStates(). Current state not recognised");
                nextState = EnemyState.idling;
                break;
        }

        // If a state transition has occurred, update the current state
        if (currentState != nextState)
            currentState = nextState;
    }

    // Have the enemy wander in some radius around a set point
    private void Idle()
    {
        enemyNavMeshAgent.speed = parameters.maximumSpeed * parameters.idlingMovementPenalty;
        
        // If the idling wander timer has expired, wander to a new point
        if (idlingTimer <= 0)
        {
            // Get a new random position in some radius around the enemy's spawn point
            Vector3 randomPosition = (Random.insideUnitSphere * parameters.idlingRadius) + idlePoint;
            // Use the SamplePosition() function to get the nearest reachable point in an area around the random position
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPosition, out hit, parameters.idlingRadius, NavMesh.AllAreas);
            // Set the enemy destination to the new position
            enemyNavMeshAgent.destination = hit.position;
            // Reset the idling wander timer
            idlingTimer = parameters.maximumIdlingTimer;

        }
    }

    private void IdlingTransitions()
    {
        // If the player is detected, chase them
        if (isPlayerDetected)
        {
            nextState = EnemyState.chasing;
        }
    }

    private void Chase()
    {
        // Run towards the player's position
        enemyNavMeshAgent.speed = parameters.maximumSpeed;
        enemyNavMeshAgent.isStopped = false;
        enemyNavMeshAgent.destination = parameters.player.position;
    }

    private void ChasingTransitions()
    {
        // If the player is no longer detected, go to the hunting state
        if (!isPlayerDetected)
        {
            lastKnownPosition = parameters.player.position;
            nextState = EnemyState.hunting;
        }
        // If the player is in attack range and the attack cooldown has expired, go to the attacking state
        if (parameters.IsPlayerInRange(parameters.attackRangeMelee, enemy) && attackTimer <= 0)
        {
            isAttacking = true;
            nextState = EnemyState.attacking;
        }
    }

    private void Hunt()
    {
        // Go to the player's last known position
        enemyNavMeshAgent.speed = parameters.maximumSpeed * parameters.huntingMovementPenalty;
        enemyNavMeshAgent.isStopped = false;
        enemyNavMeshAgent.destination = lastKnownPosition;
    }

    private void HuntingTransitions()
    {
        // If the player is detected again, return to chasing
        if (isPlayerDetected)
        {
            nextState = EnemyState.chasing;
        }
        // Return to idling if the enemy has reached the player's last known position and the player has still not been detected
        if (!isPlayerDetected && (Vector3.SqrMagnitude(enemy.position - lastKnownPosition) < 1))
        {
            nextState = EnemyState.idling;
            huntingTimer = parameters.maximumHuntingTimer;
        }
        // If the hunting timer has expired then the enemy may be unable to reach lastKnownPosition, return to idling
        if (huntingTimer <= 0)
        {
            nextState = EnemyState.idling;
            huntingTimer = parameters.maximumHuntingTimer;
        }
        else
        {
            huntingTimer -= Time.deltaTime;
        }
    }

    private void Attack()
    {
        // Stop the enemy movement until the attack is complete
        enemyNavMeshAgent.isStopped = true;

        // After a windup delay, execute the attack
        if (attackWindupTimer <= 0)
        {
            // Wait for the winddown timer to expire before resuming movement
            if (attackWinddownTimer <= 0)
            {
                // Reset timers to resume movement
                attackTimer = parameters.maximumAttackTimerMelee;
                attackWindupTimer = parameters.maximumMeleeAttackWindupTimer;
                attackWinddownTimer = parameters.maximumMeleeAttackWinddownTimer;
                // Clear the isAttacking flag
                isAttacking = false;
            }
            else
            {
                attackWinddownTimer -= Time.deltaTime;
            }
        }
        else
        {
            // Attack on the first frame of the wind up timer
            if (attackWindupTimer == parameters.maximumMeleeAttackWindupTimer)
            {
                Debug.Log("Attack");
                enemyAnimator.SetBool("isAttacking", true);
            }
            else
            {
                enemyAnimator.SetBool("isAttacking", false);
            }
            Debug.Log("WindingUp");
            // Decrement windup timer
            attackWindupTimer -= Time.deltaTime;
        }
    }

    private void AttackingTransitions()
    {
        // Once the attack is finished, return to chasing is the player is detected, or hunting otherwise
        if (!isAttacking)
        {
            nextState = (isPlayerDetected) ? (EnemyState.chasing) : (EnemyState.hunting);
            attackTimer = parameters.maximumAttackTimerMelee;
        }
    }

    // Reduce the time on all timers by deltaTime (only if they have not expired)
    private void CountDown()
    {
        if (idlingTimer > 0)
            idlingTimer -= Time.deltaTime;
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
    }
}