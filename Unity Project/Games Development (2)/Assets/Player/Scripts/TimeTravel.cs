using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTravel : MonoBehaviour
{

    //Relevant animators for TT model and flash opacity
    public Animator ttTransition;
    public Animator Flash;

    public Transform Player;

    //Controlling which timeframe player is in and cooldown (or reload time)
    public bool inPast;
    public float TimeTravelCD = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Decrement cooldown to ensure it cannot be fired early
        TimeTravelCD -= Time.deltaTime;
        
        //When timetravel initiated:
        if (Input.GetKeyDown(KeyCode.F) && inPast == false)
        {
            try
            //Sync health of bosses
            {GameObject.Find("Boss_Past").GetComponent<Bosses>().health = GameObject.Find("Boss_Present").GetComponent<Bosses>().health;}
            catch{}
            //When TT "Reloaded":
            if (TimeTravelCD <= 0)
            {
            //Identify correct direction of travel and set animators with correct delays
            inPast = true;
            Invoke("TravellingtoPast", 0.5f);
            ttTransition.SetBool("TTT", true);
            TimeTravelCD = 2f;
            }
        }

        //Same as above, for opposite direction of travel.
        else if (Input.GetKeyDown(KeyCode.F) && inPast == true)
        {
            try
            {
                GameObject.Find("Boss_Present").GetComponent<Bosses>().health = GameObject.Find("Boss_Past").GetComponent<Bosses>().health;
            }
            catch
            {}
            if (TimeTravelCD <= 0)
            {
                
                inPast = false;
                Invoke("TravellingtoPresent", 0.5f);
                ttTransition.SetBool("TTT", true);
                TimeTravelCD = 2f;
            }
        }
        
    }
    
    void TravellingtoPast()
    {
        //Move player and begin flash (timings chosen to perfectly sync effects)
        Debug.Log("TRAVELLING");
        gameObject.transform.position = new Vector3(transform.localPosition.x - 1900f, transform.localPosition.y, transform.localPosition.z);
        Flash.SetTrigger("TTFlashTrigger");
        Invoke("TTDelay", 0.1f);    //Reset variables to prevent animation looping

    }

    void TravellingtoPresent() {
        //Same as above for opposite direction of travel (Duplicate Code)
        Debug.Log("TRAVELLING");
        gameObject.transform.position = new Vector3(transform.localPosition.x + 1900f, transform.localPosition.y, transform.localPosition.z);
        Flash.SetTrigger("TTFlashTrigger");
        Invoke("TTDelay", 0.1f);
    }
    

    void TTDelay()
    {
        //Reset animation triggers
        ttTransition.SetBool("TTT", false);

        Flash.ResetTrigger("TTFlashTrigger");
    }
    
}
