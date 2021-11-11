using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hubTrigger : MonoBehaviour
{

    public GameObject wrench;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == wrench.name)
        {
            wrench.GetComponent<Rigidbody>().isKinematic = true;
        }

    }

    void onTriggerStay(Collider collider)
    {

    }

    void onTriggerExit(Collider collider)
    {

    }

}
