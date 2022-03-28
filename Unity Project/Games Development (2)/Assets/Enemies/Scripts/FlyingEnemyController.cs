using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  A script to control the flying enemies, inherits from the generic EnemyController script
 */

[RequireComponent(typeof(Rigidbody))]
public class FlyingEnemyController : EnemyController
{
    private Rigidbody enemyRigidbody;
    private Manager manager;
    private EnemyParameters parameters;
    private Transform enemy;
    public FlyingAttack projectilePrefab;
    public float attackOffset = 0.75f;

    // A list to hold a reference to all of the neighbours of this enemy
    private List<EnemyController> neighbours = new List<EnemyController>();
    private List<EnemyController> flyingNeighbours = new List<EnemyController>();

    private bool isPlayerDetected;

    private Vector3 velocity, acceleration = Vector3.zero;
    private float speed, turningTime;

    private Vector3 idlePoint;

    private Vector3 previousAvoidanceVector = Vector3.zero;

    private Vector3 lastKnownPosition = Vector3.zero;
    private float huntingTimer = 0;

    bool isInAttackRange = false;
    [SerializeField] private float attackTimer = 0;
    [SerializeField] private float attackWindupTimer = 0;
    [SerializeField] private float attackWinddownTimer = 0;

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
        // Disable the enemy
        this.gameObject.SetActive(false);
        // Remove this enemy from its corresponding enemy manager
        manager.enemies.Remove(this);
    }

    // A function to initialise various elements of the script. Called from the start() function of EnemyManager when the enemy is spawned
    public override void Initialise(Manager _manager, EnemyParameters Parameters, Vector3 SpawnPoint)
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        parameters = Parameters;
        enemy = this.transform;
        manager = _manager;
        speed = parameters.maximumSpeed;
        velocity = enemy.forward * speed;
        idlePoint = parameters.flyingIdlePoint.position;
        huntingTimer = parameters.maximumHuntingTimer;
        attackWindupTimer = parameters.maximumFlyingAttackWindupTimer;
        attackWinddownTimer = parameters.maximumFlyingAttackWinddownTimer;
    }

    // Called every frame by the enemy manager
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
        if (isPlayerDetected && parameters.IsPlayerInRange(parameters.attackRangeFlying, enemy))
            isInAttackRange = true;

        // Check whether the enemy is in attack range
        if (isInAttackRange && (attackTimer <= 0))
        {
            Attack();
        }
        // Flying enemies should only move when not attacking
        else
        {
            // Separation steering behaviour to prevent colliding with other enemies
            Vector3 separationVector = GetSeparationVector() * parameters.separationWeight;

            // Obstacle avoidance to prevent colliding with terrain
            Vector3 avoidanceVector = GetAvoidanceVector() * parameters.avoidanceWeight;

            // Variable behaviour set by FSM
            Vector3 stateMachineBehaviourVector = GetStateBehaviourVector();

            // A hovering vector which aims to keeps the agent within some range in the y-axis
            Vector3 hoveringVector = GetHoveringVector() * parameters.hoveringWeight;

            // Sum the steering behaviour contributions
            Vector3 desiredVelocity = separationVector + avoidanceVector + stateMachineBehaviourVector;

            velocity = enemy.forward * speed;
            velocity = Vector3.ClampMagnitude(velocity + desiredVelocity * Time.deltaTime, speed * parameters.flyingSpeedPenalty);
            // Turn the enemy to face the new velocity
            enemy.forward = velocity.normalized;
            enemy.position += velocity * Time.deltaTime;
        }
        // Decrement attack timer
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
    }


    /*  State Actions  */

    private void Attack()
    {
        // After a windup delay, execute the attack
        if (attackWindupTimer <= 0)
        {
            enemyRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
            // Wait for the winddown timer to expire before resuming movement
            if (attackWinddownTimer <= 0)
            {
                // Reset timers to resume movement
                attackTimer = parameters.maximumAttackTimerFlying;
                attackWindupTimer = parameters.maximumFlyingAttackWindupTimer;
                attackWinddownTimer = parameters.maximumFlyingAttackWinddownTimer;
            }
            else
            {
                attackWinddownTimer -= Time.deltaTime;
            }
        }
        else
        {
            // Spawn a projectile on the first frame of the windup timer
            if (attackWindupTimer == parameters.maximumFlyingAttackWindupTimer)
            {
                Debug.Log("Attack");
                Vector3 attackPosition = enemy.forward * attackOffset;
                Quaternion attackRotation = Quaternion.LookRotation((parameters.player.position - enemy.position).normalized, Vector3.up);
                FlyingAttack projectile = Instantiate(projectilePrefab, (enemy.position + attackPosition), enemy.rotation, transform);
                projectile.Initialise(attackWinddownTimer, parameters);
            }
            // Only follow the player's position for the firsst 75% of the windup
            if (attackWindupTimer >= parameters.maximumFlyingAttackWindupTimer * 0.25)
            {
                // Look at player
                enemy.rotation = Quaternion.LookRotation((parameters.player.position - enemy.position).normalized, Vector3.up);
            }
            Debug.Log("WindingUp");
            // Freeze enemy in place
            enemyRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            velocity = Vector3.zero;
            // Decrement windup timer
            attackWindupTimer -= Time.deltaTime;
        }
    }

    // When in the idle state, the enemy should behave like a boid, crowding with nearby idle enemies
    private Vector3 Idle()
    {
        Vector3 desiredVelocity = Vector3.zero;

        speed = parameters.maximumSpeed * parameters.idlingMovementPenalty;
        //turningTime = parameters.minimumTurningTime / parameters.idlingMovementPenalty;

        // Align with and cohere to nearby enemies (separation is handled independently of the FSM)
        desiredVelocity += GetCohesionVector() * parameters.cohesionWeight;
        desiredVelocity += GetAlignmentVector() * parameters.alignmentWeight;
        // Keep idling enemies within a certain radius
        desiredVelocity += (GetBoundsVector(idlePoint, parameters.idlingRadius) * parameters.boundsWeight);
        return desiredVelocity;
    }

    // Transition to chasing if the player is detected
    private void IdlingTransitions()
    {
        nextState = EnemyState.idling;

        if (isPlayerDetected)
        {
            nextState = EnemyState.chasing;
        }
    }

    // Apply a basic seeking steering behaviour to have the enemy chase after the player
    private Vector3 Chase()
    {
        Vector3 desiredVelocity = Vector3.zero;
        speed = parameters.maximumSpeed;
        
        desiredVelocity += GetSeekingVector(parameters.player);

        return desiredVelocity;
    }
    
    // If the enemy loses the player while chasing, they got to a hunting state, taking note of the player's last known position
    private void ChasingTransitions()
    {
        if (!isPlayerDetected)
        {
            lastKnownPosition = parameters.player.position;
            nextState = EnemyState.hunting;
        }
    }

    // The hunting state has the enemy crowd with any nearby flying enemies around the player's last known position
    private Vector3 Hunt()
    {
        Vector3 desiredVelocity = Vector3.zero;

        speed = parameters.maximumSpeed * parameters.huntingMovementPenalty;
        //turningTime = parameters.minimumTurningTime / parameters.idlingMovementPenalty;

        // Align with and cohere to nearby enemies (separation is handled independently of the FSM)
        desiredVelocity += GetCohesionVector() * parameters.cohesionWeight;
        desiredVelocity += GetAlignmentVector() * parameters.alignmentWeight;
        // Keep idling enemies within a certain radius
        desiredVelocity += (GetBoundsVector(lastKnownPosition, parameters.huntingRadius) * parameters.boundsWeight);

        return desiredVelocity;
    }

    // If the player is found while the enemy is hunting return to chasing, else go to idling after some delay
    private void HuntingTransitions()
    {
        // If the player is detected again, return to chasing
        if (isPlayerDetected)
        {
            huntingTimer = parameters.maximumHuntingTimer;
            nextState = EnemyState.chasing;
        }
        // If the hunting timer has expired then return to idling
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

    /*  Steering Behaviours  */

    // Variable steering behaviour determined by the current state of the FSM
    private Vector3 GetStateBehaviourVector()
    {
        Vector3 desiredVelocity = Vector3.zero;

        switch (currentState)
        {
            case EnemyState.idling:
                desiredVelocity = Idle();
                IdlingTransitions();
                break;
            case EnemyState.chasing:
                desiredVelocity = Chase();
                ChasingTransitions();
                break;
            case EnemyState.hunting:
                desiredVelocity = Hunt();
                HuntingTransitions();
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
                if (Vector3.SqrMagnitude(enemy.position - neighbour.position) < (parameters.separationRange * parameters.separationRange))
                {
                    desiredVelocity += (enemy.position - neighbour.position);
                    neighboursInFOV++;
                }
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
        Vector3 desiredVelocity = Vector3.zero;
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

    // The avoidance behaviour steers the agent to avoid obstacles
    private Vector3 GetAvoidanceVector()
    {
        Vector3 avoidanceVector = Vector3.zero;

        // Check for obstacles with an 'avoidanceRange' length raycast over the obstacle mask
        RaycastHit hit;
        if (Physics.Raycast(enemy.position, enemy.forward, out hit, parameters.perceptionRange, parameters.obstacleLayerMask))
        {
            // If an obstacle was detercted, return a steering vector poiting away from the obstacle
            avoidanceVector = AvoidObstacle();
        }
        // If no collision was detected, set the previousAvoidanceVector variable (used later) to the zero vector, then return the zero vector
        previousAvoidanceVector = avoidanceVector;

        return avoidanceVector;

    }

    // A function to determine the best direction to avoid an obstacle, then steer in that direction
    private Vector3 AvoidObstacle()
    {
        Vector3 selectedDirection = Vector3.zero;

        // If a non-zero avoidance vector has already been calculated check whether this one is still good before recalculating
        if (previousAvoidanceVector != Vector3.zero)
        {
            // Raycast along the previousAvoidanceVector. If nothing is hit, then the direction is still obstacle free. Return that direction
            RaycastHit hit;
            if (!Physics.Raycast(enemy.position, enemy.forward, out hit, parameters.perceptionRange, parameters.obstacleLayerMask))
            {
                selectedDirection = previousAvoidanceVector;
                return selectedDirection;
            }
        }

        // The furthestDistance variable is used to determine which direction can be travelled in the furtherst before an obstacle is hit
        float furthestDistance = int.MinValue;
        // Go through each direction in avoidanceRayDirections[] to find the ray that travels the furthest without collision
        for (int i = 0; i < parameters.avoidanceRayDirections.Length; i++)
        {
            // Raycast in the [i]th direction
            RaycastHit hit;
            Vector3 currentDirection = transform.TransformDirection(parameters.avoidanceRayDirections[i].normalized);
            if (Physics.Raycast(enemy.position, currentDirection, out hit, parameters.perceptionRange, parameters.obstacleLayerMask))
            {
                // If the hit is further away than the previous furthest hit, select this direction as the new best direction
                float currentDistance = (hit.point - enemy.position).sqrMagnitude;
                if (currentDistance > furthestDistance)
                {
                    furthestDistance = currentDistance;
                    selectedDirection = currentDirection;
                }
            }
            else
            {
                // If the raycast hit nothing, select this direction and do not check any others
                selectedDirection = currentDirection;
                return selectedDirection.normalized;
            }
        }
        return selectedDirection.normalized;
    }

    // Seeking steering behaviour
    private Vector3 GetSeekingVector(Transform target)
    {
        Vector3 desiredVelocity = Vector3.zero;

        desiredVelocity += (target.position - enemy.position);

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

    // Hovering steering behaviour (only acts in the y-direction). This behaviour steers the player up if below some threshold, or down if above another
    private Vector3 GetHoveringVector()
    {
        Vector3 desiredVelocity = Vector3.zero;

        if (enemy.position.y < parameters.lowerHoveringBound)
        {
            desiredVelocity.y = 1;
        }

        if (enemy.position.y > parameters.upperHoveringBound)
        {
            desiredVelocity.y = -1;
        }

        return desiredVelocity;
    }
}