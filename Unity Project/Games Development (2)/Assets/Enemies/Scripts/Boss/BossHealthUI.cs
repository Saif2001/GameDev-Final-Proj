using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthUI : MonoBehaviour
{

    //Transforms and relevant variables
    public Transform HealthBar;
    public float currentHealth;
    public int maxHealth = 1000;


    // Update is called once per frame
    void Update()
    {
        //Lowest health is true for both bosses.
        currentHealth = (Mathf.Min(GameObject.Find("Boss_Present").GetComponent<Bosses>().health, GameObject.Find("Boss_Past").GetComponent<Bosses>().health)) - 20;
        //Relevant transformations applied to health bar to fit correctly.
        HealthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentHealth);

    }
}
