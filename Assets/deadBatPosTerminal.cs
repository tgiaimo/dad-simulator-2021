using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class deadBatPosTerminal : MonoBehaviour
{
    public GameObject expectedClip1;
    public GameObject expectedClip2;
    public GameObject clipped;

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
        if((other.gameObject.name == expectedClip1.name) || (other.gameObject.name == expectedClip2.name))
        {
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if((other.gameObject.name == expectedClip1.name)||(other.gameObject.name == expectedClip2.name))
        {
            other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void checkClipped(GameObject x)
    {


    }

}
