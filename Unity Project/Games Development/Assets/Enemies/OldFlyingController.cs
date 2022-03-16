using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Completely broken right now 
 */

[RequireComponent(typeof(Rigidbody))]
public class OldFlyingController : EnemyController
{
    private Rigidbody enemyRigidbody;
    private EnemyManager manager;
    private EnemyParameters parameters;
    private Transform enemy;

    // A list to hold a reference to all of the neighbours of this enemy
    private List<EnemyController> neighbours = new List<EnemyController>();
    private List<EnemyController> flyingNeighbours = new List<EnemyController>();

    private bool isPlayerDetected;

    private Vector3 velocity, acceleration = Vector3.zero;
    private float speed, turningTime;

    [SerializeField] private EnemyState currentState = EnemyState.idling;
    private EnemyState nextState = EnemyState.idling;

    // A function to return the type of this enemy. Used to determine whether two neighbours are of the same type
    public override string Type()
    {
        return "Flying";
    }

    // A function that is called any time this enemy is hit by the player
    public override void GetHit()
    {
        Debug.Log("Ouch!");
        this.gameObject.SetActive(false);
        manager.enemies.Remove(this);
    }

    // A function to initialise various elements of the script. Called from the start() function of EnemyManager when the enemy is spawned
    public override void Initialise(EnemyManager Manager, EnemyParameters Parameters, Vector3 SpawnPoint)
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        parameters = Parameters;
        enemy = this.transform;
        manager = Manager;
        speed = parameters.maximumSpeed;
        velocity = enemy.forward * speed;
    }

    public override void Move(List<EnemyController> Neighbours)
    {
        // Update the neighbour list
        neighbours = Neighbours;
        // Loop through all neighbours to check which ones are also flying enimies
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours[i].Type() == "Flying")
            {
                flyingNeighbours.Add(neighbours[i]);
            }
        }

        // Check whether the player is in line of sight of this enemy
        isPlayerDetected = parameters.DetectPlayer(enemy);

        // Separation steering behaviour to prevent colliding with other enemies
        Vector3 separationVector = GetSeparationVector() * parameters.separationWeight;

        // Obstacle avoidance to prevent colliding with terrain
        Vector3 avoidanceVector = GetAvoidanceVector() * parameters.avoidanceWeight;

        // Variable behaviour set by FSM
        Vector3 stateMachineBehaviourVector = GetStateBehaviourVector();

        // Sum the steering behaviour contributions
        Vector3 desiredVelocity = separationVector + avoidanceVector + stateMachineBehaviourVector;
        Debug.DrawRay(enemy.position, desiredVelocity);
        // Use the SmoothDamp functions to smoothly steer from the current direction (transform.forward) to the desiredVelocity in some time invsersely proportional to the turningSpeed parameter
        velocity = Vector3.SmoothDamp(velocity, desiredVelocity, ref acceleration, turningTime);
        // Turn the enemy to face the new velocity
        enemy.forward = velocity.normalized;
        // Apply the velocity to the rigidbody
        enemyRigidbody.velocity = enemy.forward * speed;
    }


    /*  State Actions  */

    // When in the idle state, the enemy should behave like a boid, crowding with nearby idle enemies
    private Vector3 Idle()
    {
        Vector3 desiredVelocity = Vector3.zero;

        speed = parameters.maximumSpeed * parameters.idlingMovementPenalty;
        turningTime = parameters.minimumTurningTime / parameters.idlingMovementPenalty;

        // Align with and cohere to nearby enemies (separation is handled independently of the FSM)
        //desiredVelocity += GetCohesionVector() * parameters.cohesionWeight;
        desiredVelocity += (GetAlignmentVector() * parameters.alignmentWeight);
        // Keep idling enemies within a certain radius
        desiredVelocity += (GetBoundsVector(parameters.flyingIdlePoint.position, parameters.idlingRadius) * parameters.boundsWeight);
        return desiredVelocity;
    }

    private void IdleTransitions()
    {
        nextState = EnemyState.idling;
    }

    private Vector3 Chase()
    {
        Vector3 desiredVelocity = Vector3.zero;
        return desiredVelocity;
    }

    private Vector3 Hunt()
    {
        Vector3 desiredVelocity = Vector3.zero;
        return desiredVelocity;
    }

    /*  Steering Behaviours  */

    // Variable steering behaviour determined by the current state of the FSM
    private Vector3 GetStateBehaviourVector()
    {
        Vector3 desiredVelocity = Vector3.zero;

        switch (currentState)
        {
            case EnemyState.idling:
                desiredVelocity = Idle();
                IdleTransitions();
                break;
            case EnemyState.chasing:
                break;
            case EnemyState.hunting:
                break;
            default:
                Debug.Log("Error in FlyingEnemyController.cs: GetStateBehaviourVector(). Current state not recognised");
                nextState = EnemyState.idling;
                break;
        }

        // If a state transition has occurred, update the current state
        if (currentState != nextState)
            currentState = nextState;

        return desiredVelocity;
    }

    // Separation steering behaviour
    private Vector3 GetSeparationVector()
    {
        // Return a zero vector if there are no neighbours to steer away from
        Vector3 desiredVelocity = Vector3.zero;
        if (neighbours.Count == 0) { return desiredVelocity; }

        // For each neighbour, get the direction vector pointing from the neighbour to the agent. Average these values
        int neighboursInFOV = 0;
        for (int i = 0; i < neighbours.Count; i++)
        {
            Transform neighbour = neighbours[i].gameObject.transform;
            if (parameters.IsInFOV(enemy, neighbour))
            {
                desiredVelocity += (enemy.position - neighbour.position);
                neighboursInFOV++;
            }
        }
        desiredVelocity /= neighboursInFOV;
        // Return the normalised, average separation vector
        desiredVelocity = desiredVelocity.normalized;
        return desiredVelocity;
    }

    // Cohesion steering behaviour
    private Vector3 GetCohesionVector()
    {
        // If there are no neighbours, return a zero vector
        Vector3 desiredVelocity = Vector3.zero;
        if (flyingNeighbours.Count == 0) { return desiredVelocity; }

        // Sum the positions of each neighobur within FOV. Count the number of points summed
        int neighboursInFOV = 0;
        for (int i = 0; i < flyingNeighbours.Count; i++)
        {
            Transform neighbour = flyingNeighbours[i].gameObject.transform;
            if (parameters.IsInFOV(enemy, neighbour))
            {
                desiredVelocity += neighbour.position;
                neighboursInFOV++;
            }
        }
        // Divide by number of points to get the average position
        desiredVelocity /= neighboursInFOV;
        // Return the normalsed direction vector to the centre point
        desiredVelocity -= enemy.position;
        desiredVelocity = desiredVelocity.normalized;
        return desiredVelocity;
    }

    // Alignment steering behaviour
    private Vector3 GetAlignmentVector()
    {
        // If there are no neighoburs, return the forward vector to maintain current course
        Vector3 desiredVelocity = transform.forward;
        if (flyingNeighbours.Count == 0) { return desiredVelocity; };

        // Sum the transform.forward vectors of each neighbour within FOV and take average
        int neighboursInFOV = 0;
        for (int i = 0; i < flyingNeighbours.Count; i++)
        {
            Transform neighbour = flyingNeighbours[i].gameObject.transform;
            if (parameters.IsInFOV(enemy, neighbour))
            {
                desiredVelocity += neighbour.forward;
                neighboursInFOV++;
            }
        }
        desiredVelocity /= neighboursInFOV;
        // Return the normalised average heading
        desiredVelocity = desiredVelocity.normalized;
        return desiredVelocity;
    }

    // Obstacle avoidance steering behaviour
    private Vector3 GetAvoidanceVector()
    {
        Vector3 desiredVelocity = Vector3.zero;
        return desiredVelocity;
    }

    // Seeking steering behaviour
    private Vector3 GetSeekingVector(Transform target)
    {
        Vector3 desiredVelocity = Vector3.zero;
        return desiredVelocity;
    }

    // Keep the enemy within some radius about a certain point
    private Vector3 GetBoundsVector(Vector3 centrePoint, float radius)
    {
        // Get the direction vector to the centre of the bounds
        Vector3 offsetToCentre = centrePoint - enemy.position;
        // If the agent is outwith the bounds, return the normalised direction vector to steer it back towards the centre
        if (offsetToCentre.sqrMagnitude > (radius * radius))
        {
            offsetToCentre = offsetToCentre.normalized;
            return offsetToCentre;
        }
        // If the agent is within the bounds, return a zero vector
        return Vector3.zero;
    }

    // Hovering steering behaviour (only acts in the y-direction)
    private Vector3 GetHoveringVector()
    {
        Vector3 desiredVelocity = Vector3.zero;
        return desiredVelocity;
    }
}
