using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    int moveSpeed = 10;
    public float  speedMult = 1;

    public void speedMultiplier()
    {
        speedMult = 0.1f;
      
        Invoke("powerupRevert", 4);

    }

    void powerupRevert()
    {
        speedMult = 1;
    }

    void Update()
    {

        //Different axes according to Unity's input manager
        bool moveHor = Input.GetButton("Horizontal");
        bool moveFwd = Input.GetButton("Vertical");

        float directionHor = Input.GetAxis("Horizontal");
        float directionFwd = Input.GetAxis("Vertical");

        bool jump = Input.GetButtonDown("Jump");

        if (moveFwd)
        {
            //Forward and backward movement
            if (directionFwd > 0)
            {
                transform.Translate(moveSpeed * speedMult * Vector3.forward * Time.deltaTime);
            }
            if (directionFwd < 0)
            {
                transform.Translate(moveSpeed * speedMult * Vector3.back * Time.deltaTime);
            }
        }


        if (moveHor)
        {
            //Left and Right movement
            if (directionHor > 0)
            {
                transform.Translate(moveSpeed * speedMult * Vector3.right * Time.deltaTime);
            }
            if (directionHor < 0)
            {
                transform.Translate(moveSpeed * speedMult * Vector3.left * Time.deltaTime);
            }
        }

        if (jump)
        {
            //Addforce is more organic than translating in y direction

            transform.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 2000f, 0f));

        }

    }
}