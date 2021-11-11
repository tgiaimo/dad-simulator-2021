using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerFL : MonoBehaviour
{
    public GameObject flat;
    public GameObject good;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == flat.name || collider.gameObject.name == good.name)
        {
            collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.name == flat.name || collider.gameObject.name == good.name)
        {
            collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        if(collider.gameObject.name == flat.name)
        {
            collider.gameObject.GetComponent<badTireCheck>().off = false;
        }

        if(collider.gameObject.name == good.name)
        {
            collider.gameObject.GetComponent<goodTireCheck>().on= true;
        }

    }

    void OnTriggerExit(Collider collider)
    {
        if(collider.gameObject.name == flat.name)
        {
            collider.gameObject.GetComponent<badTireCheck>().off = true;
        }
    }

}
