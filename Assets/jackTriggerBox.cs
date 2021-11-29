using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class jackTriggerBox : MonoBehaviour
{
    public GameObject jack;
    public GameObject rightHand;
    public GameObject leftHand;
    public Transform endPos;
    public Transform startPos;
    public float sped;
    public float desiredDuration;
    private float elapsedTime=0;
    //public bool schmoved = false;

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
        if (other.gameObject.transform.parent.gameObject.transform.parent.gameObject.name == jack.name)
        {
            startPos = other.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform;
            if (rightHand.GetComponent<Hand>().ObjectIsAttached(jack))
            {
                rightHand.GetComponent<Hand>().DetachObject(jack);
            }
            if (leftHand.GetComponent<Hand>().ObjectIsAttached(jack))
            {
                leftHand.GetComponent<Hand>().DetachObject(jack);
            }
            //other.gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine(Move());
            GameObject.Find("jackDetailedHalo").GetComponent<jackCheck>().inLocation = true;
            GameObject test = this.gameObject;
            test.GetComponent<MeshRenderer>().enabled = false;
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


    IEnumerator Move()
    {
        while (elapsedTime < desiredDuration)
        {
            jack.transform.position=Vector3.Lerp(startPos.position, endPos.position, (elapsedTime/desiredDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //jack.GetComponent<Rigidbody>().constraints= RigidbodyConstraints.FreezeAll;
        Debug.Log("schmoovin");
        yield return null;
    }
}