using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthUI : MonoBehaviour
{

    public Transform HealthBar;
    public float currentHealth;
    public int maxHealth = 1000;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = (Mathf.Min(GameObject.Find("Boss_Present").GetComponent<Bosses>().health, GameObject.Find("Boss_Past").GetComponent<Bosses>().health)) - 20;
        //HealthBar.localScale = (currentHealth/maxHealth);
        HealthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentHealth);//*(maxHealth/1000));

    }
}
