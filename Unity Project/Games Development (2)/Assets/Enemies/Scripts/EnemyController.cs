using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  This is an abstract class from which enemy controllers (melee, ranged, flying) inherit
 *  Each abstact function in this class will be overriden with a separate body of code in each of the controller versions
 */

abstract public class EnemyController : MonoBehaviour
{
    abstract public void Initialise(Manager Manager, EnemyParameters Parameters, Vector3 SpawnPosition);

    abstract public void Move(List<EnemyController> Neighbours);

    abstract public void GetHit();

    abstract public string Type();
}
