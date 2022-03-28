using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTriggerBehaviour : MonoBehaviour
{

    //Function to enable boss and boss components on arena entry. Disables components before they are needed.
    public GameObject BossHealthBarUI;
    private void Start()
    {
        //Disable boss script (prevent shooting etc) of both bosses
        GameObject.Find("Boss_Present").GetComponent<Bosses>().enabled = false;
        GameObject.Find("Boss_Past").GetComponent<Bosses>().enabled = false;

        //Disable Boss health
        BossHealthBarUI.SetActive(false);
        
        Debug.Log("Scripts disabled");
    }
    // Start is called before the first frame update
    private void OnTriggerExit(Collider other)
    {

        //When trigger passed through, re-enable all boss components (Scripts, healthbars)
        GameObject.Find("Boss_Present").GetComponent<Bosses>().enabled = true;
        GameObject.Find("Boss_Past").GetComponent<Bosses>().enabled = true;
        BossHealthBarUI.SetActive(true);

        //Block player in room with boss until death
        GameObject.Find("Block_Backward").transform.position = (new Vector3(453.2f, 201, -2111));
        GameObject.Find("Block_Forward").transform.position = (new Vector3(788.45f, 190, -2104.44f));
    }
}
