using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jackInteraction : MonoBehaviour
{
    public GameObject yurt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void raised()
    {
        yurt.GetComponent<jackCheck>().raised = true;
        yurt.GetComponent<jackCheck>().lowered= false;
    }

    public void unraised()
    {
        yurt.GetComponent<jackCheck>().raised = false;
        yurt.GetComponent<jackCheck>().lowered= true;
    }
}
