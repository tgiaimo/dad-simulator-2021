using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class crankTrigger : MonoBehaviour
{
    public GameObject crank;
    public GameObject trigger;
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
        if (other.gameObject.name == crank.name)
        {
            if (rightHand.GetComponent<Hand>().ObjectIsAttached(crank))
            {
                rightHand.GetComponent<Hand>().DetachObject(crank);
            }
            if (leftHand.GetComponent<Hand>().ObjectIsAttached(crank))
            {
                leftHand.GetComponent<Hand>().DetachObject(crank);
            }
            crank.GetComponent<Rigidbody>().isKinematic = true;
            Destroy(crank.GetComponent<Throwable>());
            crank.GetComponent<CircularDrive>().enabled = true;
            trigger.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
