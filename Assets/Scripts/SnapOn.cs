using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapOn : MonoBehaviour
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
        if (other.gameObject.CompareTag("snap"))
        {
            var parentcomp = gameObject.GetComponentInParent(typeof(Rigidbody)) as Rigidbody;
            parentcomp.isKinematic = true;
        }
    }

}
