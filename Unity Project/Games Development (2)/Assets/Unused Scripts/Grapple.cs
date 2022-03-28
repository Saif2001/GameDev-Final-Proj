using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{

   

    public Transform Player;
    public Transform fpsCamera;

    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            if(Physics.Raycast(fpsCamera.position, (fpsCamera.forward), out hit))
            {
                transform.Translate(fpsCamera.forward, Space.World);
                Debug.Log(hit.point);
            }
        }
    }
}
