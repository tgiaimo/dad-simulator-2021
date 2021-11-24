using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class jackTriggerBox : MonoBehaviour
{
    public GameObject jack;
    public Vector3 startPos;
    public Vector3 endPos=new Vector3((float)40.4160004,(float)-6.86299992,(float)15.2460003);

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
        if (other.gameObject.name == jack.name)
        {
            startPos = jack.transform.position;
            StartCoroutine(Move());
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            GameObject.Find("jackDetailedHalo").GetComponent<jackCheck>().inLocation = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == jack.name)
        {
            other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            GameObject.Find("jackDetailedHalo").GetComponent<jackCheck>().inLocation = false;
        }
    }


 IEnumerator Move(){
     float startTime=Time.time; // Time.time contains current frame time, so remember starting point
     while(Time.time-startTime<=1000){ // until one second passed
         jack.transform.position=Vector3.Lerp(startPos,endPos,Time.time-startTime); // lerp from A to B in one second
         yield return null; // wait for next frame
     }
 }
}