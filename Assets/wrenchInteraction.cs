using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class wrenchInteraction : MonoBehaviour
{
    public GameObject wrench;
    public GameObject lug;
    public GameObject rightHand;
    public GameObject leftHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == wrench.name)
        {
            wrench.GetComponent<CircularDrive>().outAngle= 0;
            wrench.GetComponent<interactions>().trigger = this.gameObject;
            lug = this.gameObject.transform.parent.gameObject;
            wrench.GetComponent<interactions>().lug = lug;
            if (rightHand.GetComponent<Hand>().ObjectIsAttached(wrench))
            {
                rightHand.GetComponent<Hand>().DetachObject(wrench);
                other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            if (leftHand.GetComponent<Hand>().ObjectIsAttached(wrench))
            {
                leftHand.GetComponent<Hand>().DetachObject(wrench);
                other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            Destroy(other.gameObject.GetComponent<Throwable>());
            other.gameObject.GetComponent<CircularDrive>().enabled=true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == wrench.name)
        {
            //other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
