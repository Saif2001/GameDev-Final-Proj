using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTriggerBehaviour : MonoBehaviour
{

    public GameObject BossHealthBarUI;
    private void Start()
    {
        GameObject.Find("Boss_Present").GetComponent<Bosses>().enabled = false;
        GameObject.Find("Boss_Past").GetComponent<Bosses>().enabled = false;
        BossHealthBarUI.SetActive(false);
        
        Debug.Log("Scripts disabled");
    }
    // Start is called before the first frame update
    private void OnTriggerExit(Collider other)
    {
        GameObject.Find("Boss_Present").GetComponent<Bosses>().enabled = true;
        GameObject.Find("Boss_Past").GetComponent<Bosses>().enabled = true;
        BossHealthBarUI.SetActive(true);

        GameObject.Find("Block_Backward").transform.position = (new Vector3(453.2f, 201, -2111));
        GameObject.Find("Block_Forward").transform.position = (new Vector3(788.45f, 190, -2104.44f));
    }
}
