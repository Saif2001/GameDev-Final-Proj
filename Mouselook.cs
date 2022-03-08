using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mouselook : MonoBehaviour
{

    public float Sensitivity = 100;

    public Transform playerObject;
    public Transform playerCam;

    float xRotation = 0f;


    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;       //Keep cursor from clicking off screen

    }


    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;         //Time.deltaTime makes mouse movement insensitive to framerate

        xRotation -= mouseY;        //Rotation around central axis
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);          //Cannot "loop" around own head with this line

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerObject.Rotate(Vector3.up * mouseX);           //Relating rotational vector to mouse movement

        float lookRotation = Vector3.SignedAngle(Vector3.up, transform.up, transform.forward);
        Quaternion inverseRotation = Quaternion.Inverse(transform.localRotation);

        if (Input.GetKeyDown(KeyCode.Mouse1))           //Ballista
        {

            playerObject.GetComponent<Rigidbody>().AddForce((-transform.forward) * 75, ForceMode.Impulse);      //Movement impulse for high speed
        }

    }
}