using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingEnemyController : MonoBehaviour
{
    public void GetHit()
    {
        this.gameObject.SetActive(false);
        Debug.Log("Ouch!");
    }
}
