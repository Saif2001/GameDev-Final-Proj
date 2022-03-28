using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    public Animator Ending;
    private void OnTriggerEnter(Collider other)
    {
        GameObject.Find("PlayerUI").SetActive(false);

        Ending.SetTrigger("EndReached");
    }

}
