using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EnemyController : MonoBehaviour
{
    abstract public void Initialise(EnemyManager Manager, EnemyParameters Parameters, Vector3 SpawnPosition);

    abstract public void Move(List<EnemyController> Neighbours);

    abstract public void GetHit();

    abstract public string Type();
}
