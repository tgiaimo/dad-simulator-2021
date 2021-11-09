using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
public class badBatPosTerminal : MonoBehaviour
{
    private bool grabbed;
    private bool insideSnapZone;
    public bool Snapped;
    private GameObject o;

    public GameObject tool;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        grabbed = GameObject.Find("LeftHand").GetComponent<Hand>().ObjectIsAttached(o);
        grabbed = GameObject.Find("RightHand").GetComponent<Hand>().ObjectIsAttached(o);
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == tool.name)
        {
            o = other.gameObject;
            insideSnapZone = true;
            o.GetComponent<Rigidbody>().isKinematic = true;
            GameObject.Find("battery/red").GetComponent<clippedCheck>().clipped = true;
        }
    }
    void OnTriggerStay(Collider other)
    {
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == tool.name)
        {
            o = other.gameObject;
            insideSnapZone = false;
            o.GetComponent<Rigidbody>().isKinematic = false;
            GameObject.Find("battery/red").GetComponent<clippedCheck>().clipped = false;
        }
    }
}
