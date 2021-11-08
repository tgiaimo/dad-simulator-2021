using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class interactions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void switchThrowCirc()
    {
        this.GetComponent<CircularDrive>().enabled = !enabled;
        this.GetComponent<ComplexThrowable>().enabled = !enabled;
    }
}
