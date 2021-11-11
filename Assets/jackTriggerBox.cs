using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jackTriggerBox : MonoBehaviour
{
    public GameObject jack;
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
        if (collider.gameObject.name == jack.name)
        {
            collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            GameObject.Find("jackDetailedHalo").GetComponent<jackCheck>().inLocation = true;
        }
    }

    void onTriggerStay(Collider collider)
    {
        if (collider.gameObject.name == jack.name)
        {
            jack.GetComponent<Rigidbody>().isKinematic = true;
            GameObject.Find("jackDetailedHalo").GetComponent<jackCheck>().inLocation = true;
        }
    }

    void onTriggerExit(Collider collider)
    {
        if (collider.gameObject.name == jack.name)
        {
            GameObject.Find("jackDetailedHalo").GetComponent<jackCheck>().inLocation = false;
        }
    }
}
