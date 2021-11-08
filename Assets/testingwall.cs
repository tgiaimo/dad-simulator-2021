using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class testingwall : MonoBehaviour
{
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
        Debug.Log("an object has been detected " + other);
        GameObject temp=other.gameObject;
        Debug.Log("object is " + temp);
        temp = temp.transform.parent.gameObject;
        Debug.Log("parent is " + temp);
        temp.GetComponent<Rigidbody>().isKinematic = true;
        if (temp.GetComponent<Interactable>().attachedToHand)
        {
            temp.GetComponent<Interactable>().attachedToHand.DetachObject(temp, false);
        }
        if (temp.GetComponent<Throwable>())
        {
            Destroy(temp.GetComponent<Throwable>());
        }
    }

    void OnTriggerStay(Collider other)
    {
        GameObject temp = other.gameObject.transform.parent.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
    }

    public void letGo(Collider other)
    {
        //trigger.enabled = false;
        GameObject temp=other.gameObject;
        temp = temp.transform.parent.gameObject;
        temp.GetComponent<Interactable>().attachedToHand.DetachObject(temp, false);
    }

    public void removeTrigger(GameObject gameObject)
    {
        gameObject.GetComponent<MeshCollider>().enabled = false;
    }

    public void makeThrowable(GameObject gameObject)
    {
        Destroy(gameObject.GetComponent<CircularDrive>());
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.AddComponent<Throwable>().enabled = true;
        gameObject.GetComponent<Throwable>().enabled = true;
    }
}

