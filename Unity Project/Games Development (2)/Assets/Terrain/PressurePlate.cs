using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] GameObject Saws;
    [SerializeField] GameObject TrapObj;
    [SerializeField] GameObject TrapMovement;
    [SerializeField] float speed = 10f;

        void Start()
        {
            TrapObj.transform.position = new Vector3(156.600006f, 129.800003f, -436.200012f);
            
        }
private void OnTriggerEnter(Collider other)
{



    if(other.gameObject.CompareTag("Player"))
    {
            //TrapObj.transform.position = new Vector3(transform.position.x, transform.position.y + 66f, transform.position.z); ;
            TrapObj.transform.position = new Vector3(158.759995f, 177.289993f, -442.339996f);
            Saws.transform.position = Vector3.MoveTowards(Saws.transform.position,TrapMovement.transform.position,speed *Time.deltaTime);
        }
}
}

