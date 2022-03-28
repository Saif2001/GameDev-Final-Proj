using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  A script to manage the first combat arena and enemies spawn by it. Inherits from the abstract Manager class
 */

[RequireComponent(typeof(EnemyParameters))]
public class ArenaController : Manager
{
    public Transform door;
    public Transform doorOpenPosition;
    public Transform doorClosedPosition;
    public float doorSpeed = 10f;

    private EnemyParameters parameters;

    public float maximumEnemies = 8f;
    public float enemySpawnTimer = 2f;

    private bool playerInArena = false;
    private bool arenaComplete = false;

    public Transform[] spawnPoints;
    public EnemyController[] enemyPrefabs;

    // When the player hits the trigger lock the arena door
    private void OnTriggerEnter(Collider other)
    {
        playerInArena = true;
    }

    // Used to reset the arena if the player dies
    public void Reset()
    {
        playerInArena = false;
        arenaComplete = false;
    }

    public void Complete()
    {
        arenaComplete = true;
    }

    void Start()
    {
        enemies = new List<EnemyController>();
        parameters = GetComponent<EnemyParameters>();
    }

    void Update()
    {
        // If the player is in the arena close the door, otherwise open it
        if (playerInArena)
        {
            door.position = Vector3.MoveTowards(door.position, doorClosedPosition.position, doorSpeed * Time.deltaTime);
        }
        else
        {
            door.position = Vector3.MoveTowards(door.position, doorOpenPosition.position, doorSpeed * Time.deltaTime);
        }

        // If the player is in the arena, and it is not yet complete, continuously spawn enemies
        if (playerInArena && !arenaComplete)
        {
            // If there are fewer than the maximum number of enemies then spawn a new one
            if (enemies.Count < maximumEnemies)
            {
                // Spawn the enemy after a delay
                Invoke("SpawnEnemy", enemySpawnTimer);
            }
        }
        
        // If there are enemies in the arena, loop through each and update them using their enemy controllers
        if (enemies.Count > 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                List<EnemyController> neighbours = GetNeighbours(enemies[i]);
                enemies[i].Move(neighbours);
            }
        }
    }

    // A function for spawning an enemy in the arena
    public void SpawnEnemy()
    {
        if (enemyPrefabs.Length > 0 && spawnPoints.Length > 0)
        {
            // If there are fewer than the maximum allowable number of enemies
            if (enemies.Count < maximumEnemies)
            {
                // Choose a random spawn point from the list
                Vector3 point = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
                // Spawn a random enemy prefab from the list at the chosen spawn point
                EnemyController enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], point, Quaternion.identity, this.transform);
                // Initialise the enemy and add it to the list of enemies managed
                enemy.Initialise(this, parameters, point);
                enemies.Add(enemy);
            }
        }
        else
        {
            Debug.Log("Error in ArenaController.cs: No enemy prefabs detected");
        }
    }

    private List<EnemyController> GetNeighbours(EnemyController enemy)
    {
        List<EnemyController> neighbours = new List<EnemyController>();

        // Loop through each possible neighbouring agent in the list
        for (int j = 0; j < enemies.Count; j++)
        {
            // Prevent an agent from neighbouring itself
            if (enemies[j] == enemy) { continue; }
            // Get the square separation and compare it to the square of perception radius. Squaring perceptionRadius is more efficient that square rooting the separation to get magnitude
            float sqrSeparation = Vector3.SqrMagnitude(enemies[j].transform.position - enemy.transform.position);
            // If the agent is within the perception radius, add it as a neighbour
            if (sqrSeparation <= parameters.perceptionRange * parameters.perceptionRange)
            {
                neighbours.Add(enemies[j]);
            }
        }
        return neighbours;
    }
}
