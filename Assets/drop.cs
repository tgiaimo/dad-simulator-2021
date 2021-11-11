using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void dropKinematic()
    {
        this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
}
