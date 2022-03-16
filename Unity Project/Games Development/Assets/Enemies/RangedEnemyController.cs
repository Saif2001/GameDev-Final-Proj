using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class RangedEnemyController : EnemyController
{
    private Rigidbody enemyRigidbody;
    private NavMeshAgent enemyNavMeshAgent;
    private EnemyManager manager;
    private EnemyParameters parameters;
    private Transform enemy;

    private EnemyState currentState = EnemyState.idling;
    private EnemyState nextState = EnemyState.idling;

    private bool isPlayerDetected = false;

    private Vector3 idlePoint;
    private float idlingTimer;

    private Vector3 lastKnownPosition;
    private float huntingTimer;

    private bool isAttacking = false;
    private float attackTimer;

    public override string Type()
    {
        return "Ranged";
    }

    // A function that is called any time this enemy is hit by the player
    public override void GetHit()
    {
        Debug.Log("Ouch!");
        this.gameObject.SetActive(false);
        manager.enemies.Remove(this);
    }

    public override void Initialise(EnemyManager Manager, EnemyParameters Parameters, Vector3 SpawnPosition)
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        parameters = Parameters;
        enemy = this.transform;
        manager = Manager;
        idlePoint = SpawnPosition;
        idlingTimer = parameters.maximumIdlingTimer;
        huntingTimer = parameters.maximumHuntingTimer;
        attackTimer = parameters.maximumAttackTimerRanged;
    }

    public override void Move(List<EnemyController> Neighbours)
    {
        // Check whether the player is in line of sight of this enemy
        isPlayerDetected = parameters.DetectPlayer(enemy);
        UpdateStates();
    }

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
                Debug.Log("Error in FlyingEnemyController.cs: GetStateBehaviourVector(). Current state not recognised");
                nextState = EnemyState.idling;
                break;
        }

        // If a state transition has occurred, update the current state
        if (currentState != nextState)
            currentState = nextState;
    }

    private void Idle()
    {
        enemyNavMeshAgent.speed = parameters.maximumSpeed * parameters.idlingMovementPenalty;

        // If the idling wander timer has expired, wander to a new point
        if (idlingTimer <= 0)
        {
            // Get a new random position in some radius around the enemy
            Vector3 randomPosition = (Random.insideUnitSphere * parameters.idlingRadius) + idlePoint;
            // Use the SamplePosition() function to get the nearest reachable point in an area around the random position
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPosition, out hit, parameters.idlingRadius, NavMesh.AllAreas);
            // Set the enemy destination to the new position
            enemyNavMeshAgent.destination = hit.position;
            // Reset the idling wander timer
            idlingTimer = parameters.maximumIdlingTimer;

        }
        idlingTimer -= Time.deltaTime;
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
        // If the player is in attack range, go to the attacking state
        if (parameters.IsPlayerInRange(parameters.attackRangeMelee, enemy))
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
        if (!isPlayerDetected && (enemy.position == lastKnownPosition))
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
    }

    private void Attack()
    {
        isAttacking = false;
    }

    private void AttackingTransitions()
    {
        // Once the attack is finished, return to chasing is the player is detected, or hunting otherwise
        if (!isAttacking)
        {
            nextState = (isPlayerDetected) ? (EnemyState.chasing) : (EnemyState.hunting);
        }
    }

    // Reduce the time on all timers by deltaTime (only if they have not expired)
    private void CountDown()
    {
        if (idlingTimer > 0)
            idlingTimer -= Time.deltaTime;
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
        if (huntingTimer > 0)
            huntingTimer -= Time.deltaTime;
    }
}