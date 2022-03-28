using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  A simple abstract script from which enemy mangers (normal, respawning, combat arena 1) inherit.
 *  Only contains a single field, a list of enemies
 */

abstract public class Manager : MonoBehaviour
{
    public List<EnemyController> enemies;
}
