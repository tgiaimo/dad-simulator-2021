using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class interactions : MonoBehaviour
{
    public GameObject yurt;
    public GameObject trigger;
    public GameObject lug;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void unTighten()
    {
        lug.gameObject.transform.GetChild(0).GetComponent<HubCheck>().tight = false;
    }
    public void makeThrow()
    {
        trigger.GetComponent<BoxCollider>().enabled = false;
        trigger.GetComponent<MeshRenderer>().enabled = false;
        yurt.GetComponent<CircularDrive>().enabled = false;
        yurt.AddComponent<Throwable>();
        yurt.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void forceKinematic()
    {
        yurt.GetComponent<Rigidbody>().isKinematic = false;
    }
}
