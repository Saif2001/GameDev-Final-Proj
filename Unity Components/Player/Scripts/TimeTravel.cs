using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTravel : MonoBehaviour
{
    public Animator ttTransition;
    public Transform Player;
    public bool inPast;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F) && inPast == false)
        {
            gameObject.transform.position = new Vector3(transform.localPosition.x - 1900f, transform.localPosition.y, transform.localPosition.z);
            inPast = true;
            TravellingtoPast();
        }
        else if (Input.GetKeyDown(KeyCode.F) && inPast == true)
        {
            gameObject.transform.position = new Vector3(transform.localPosition.x + 1900f, transform.localPosition.y, transform.localPosition.z);
            inPast = false;
            TravellingtoPresent();
        }

    }
    
    void TravellingtoPast()
    {
        Debug.Log("TRAVELLING");
        ttTransition.SetBool("TTT", true);
        Invoke("TTDelay", 1);

    }
    void TravellingtoPresent() {
        Debug.Log("TRAVELLING");
        ttTransition.SetBool("TTT", true);
        Invoke("TTDelay", 1);
    }
    



    void TTDelay()
    {
        ttTransition.SetBool("TTT", false);
    }
    
}
