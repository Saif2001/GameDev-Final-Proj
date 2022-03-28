using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTravel : MonoBehaviour
{
    public Animator ttTransition;
    public Animator Flash;
    public Transform Player;
    public bool inPast;
    public float TimeTravelCD = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TimeTravelCD -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F) && inPast == false)
        {
            try
            {
                GameObject.Find("Boss_Past").GetComponent<Bosses>().health = GameObject.Find("Boss_Present").GetComponent<Bosses>().health;
            }
            catch
            {

            }
           if (TimeTravelCD <= 0)
           {
           
           inPast = true;
           //TravellingtoPast
           Invoke("TravellingtoPast", 0.5f);
           ttTransition.SetBool("TTT", true);
           TimeTravelCD = 2f;
           }
        }

        else if (Input.GetKeyDown(KeyCode.F) && inPast == true)
        {
            try
            {
                GameObject.Find("Boss_Present").GetComponent<Bosses>().health = GameObject.Find("Boss_Past").GetComponent<Bosses>().health;

            }
            catch
            {
                Debug.Log("failed");
            }
            if (TimeTravelCD <= 0)
            {
                
                inPast = false;
                //TravellingtoPresent();
                Invoke("TravellingtoPresent", 0.5f);
                ttTransition.SetBool("TTT", true);
                TimeTravelCD = 2f;
            }
        }
        
    }
    
    void TravellingtoPast()
    {
        Debug.Log("TRAVELLING");
        gameObject.transform.position = new Vector3(transform.localPosition.x - 1900f, transform.localPosition.y, transform.localPosition.z);
        //ttTransition.SetBool("TTT", true);
        Flash.SetTrigger("TTFlashTrigger");
        Invoke("TTDelay", 0.1f);

    }
    void TravellingtoPresent() {
        Debug.Log("TRAVELLING");
        gameObject.transform.position = new Vector3(transform.localPosition.x + 1900f, transform.localPosition.y, transform.localPosition.z);
        //ttTransition.SetBool("TTT", true);
        Flash.SetTrigger("TTFlashTrigger");
        Invoke("TTDelay", 0.1f);
    }
    



    void TTDelay()
    {
        ttTransition.SetBool("TTT", false);

        Flash.ResetTrigger("TTFlashTrigger");
    }
    
}
