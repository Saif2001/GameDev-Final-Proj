using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  An enemy mangager script. Spawns enemies continuously
 */

[RequireComponent(typeof(EnemyParameters))]
public class RespawningEnemyManager : Manager
{
    EnemyParameters parameters;

    // Enemy prefabs
    public EnemyController[] enemyPrefabs;

    // Enemy spawn locations
    public Transform[] spawnPoints;

    public float maximumEnemies = 1f;

    public bool playerInArena = false;
    public bool arenaComplete = false;

    private float spawningDelay = 0f;
    public float maximumSpawningDelay = 2f;

    void Start()
    {
        parameters = GetComponent<EnemyParameters>();
        enemies = new List<EnemyController>();
    }

    // Spawn new enemies and control existing ones
    void Update()
    {
        // Only spawn enemies if the player is within the designated area
        if (playerInArena && !arenaComplete)
        {
            // Spawn enemies after a delay
            if (spawningDelay <= 0)
            {
                // If there are fewer than the maximum number of enemies then spawn a new one
                if (enemies.Count < maximumEnemies)
                {
                    spawningDelay = maximumSpawningDelay;
                    Vector3 point = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
                    EnemyController enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], point, Quaternion.identity, this.transform);
                    enemy.Initialise(this, parameters, point);
                    enemies.Add(enemy);
                }
            }
        }
        spawningDelay -= Time.deltaTime;

        // Move all existing enemies
        for (int i = 0; i < enemies.Count; i++)
        {
            // Get the neighbours of the current enemy
            List<EnemyController> neighbours = GetNeighbours(enemies[i]);
            // Call the move function (this is an abstract function and is different depending on enemy type)
            enemies[i].Move(neighbours);
        }
    }

    // Get the neighbours of the current enemy
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
