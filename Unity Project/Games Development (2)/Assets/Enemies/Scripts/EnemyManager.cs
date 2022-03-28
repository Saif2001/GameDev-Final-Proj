using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  This class is used to spawn and manage enemies in the scene
 */

[RequireComponent(typeof(EnemyParameters))]
public class EnemyManager : Manager
{
    // A list of common parameters used by enemies
    EnemyParameters parameters;

    // Enemy prefabs
    public FlyingEnemyController flyingEnemyPrefab;
    public MeleeEnemyController meleeEnemyPrefab;
    public RangedEnemyController rangedEnemyPrefab;

    // Enemy spawn locations
    public Transform[] flyingSpawns;
    public Transform[] meleeSpawns;
    public Transform[] rangedSpawns;

    // When the script starts, enemies are spawned into the scene, one enemy for each spawn point that has been added in the inspector
    void Start()
    {
        parameters = GetComponent<EnemyParameters>();
        enemies = new List<EnemyController>();
        // Spawn Enemies
        // If there are flying enemies to spawn
        if (flyingSpawns.Length != 0)
        {
            for (int i = 0; i < flyingSpawns.Length; i++)
            {
                // Spawn flying enemies
                EnemyController enemy = Instantiate(flyingEnemyPrefab, flyingSpawns[i].position, Quaternion.identity, transform);
                // Initialise the enemy script
                enemy.Initialise(this, parameters, flyingSpawns[i].position);
                enemy.name = "Flying Enemy " + i;
                // Add the new enemy to the list of all enemies
                enemies.Add(enemy);
            }
        }
        // If there are melee enemies to spawn
        if (meleeSpawns.Length != 0)
        {
            for (int i = 0; i < meleeSpawns.Length; i++)
            {
                // Spawn melee enemies
                EnemyController enemy = Instantiate(meleeEnemyPrefab, meleeSpawns[i].position, Quaternion.identity, transform);
                // Initialise the enemy script
                enemy.Initialise(this, parameters, meleeSpawns[i].position);
                enemy.name = "Melee Enemy " + i;
                // Add the new enemy to the list of all enemies
                enemies.Add(enemy);
            }
        }
        // If there are ranged enemies to spawn
        if (rangedSpawns.Length != 0)
        {
            for (int i = 0; i < rangedSpawns.Length; i++)
            {
                // Spawn ranged enemies
                EnemyController enemy = Instantiate(rangedEnemyPrefab, rangedSpawns[i].position, Quaternion.identity, transform);
                // Initialise the enemy script
                enemy.Initialise(this, parameters, rangedSpawns[i].position);
                enemy.name = "Ranged Enemy " + i;
                // Add the new enemy to the list of all enemies
                enemies.Add(enemy);
            }
        }
    }

    // Each frame, the manager loops through every enemy assigned to it and calls the Move() function to update their position/states
    void Update()
    {
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
