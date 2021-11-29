using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class crankTrigger : MonoBehaviour
{
    public GameObject crank;
    public GameObject jack;
    public GameObject trigger;
    public GameObject rightHand;
    public GameObject leftHand;
    public Transform crankDest;

    private Transform startPos;
    private float elapsedTime;
    private float desiredDuration=0.5f;

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
        if (other.gameObject.name == crank.name)
        {
            elapsedTime = 0.0f;
            startPos = crank.transform;
            if (rightHand.GetComponent<Hand>().ObjectIsAttached(crank))
            {
                rightHand.GetComponent<Hand>().DetachObject(crank);
            }
            if (leftHand.GetComponent<Hand>().ObjectIsAttached(crank))
            {
                leftHand.GetComponent<Hand>().DetachObject(crank);
            }
            jack.GetComponent<LinearAnimator>().enabled = true;
            crank.GetComponent<Rigidbody>().isKinematic = true;
            Destroy(crank.GetComponent<Throwable>());
            crank.GetComponent<CircularDrive>().enabled = true;
            trigger.GetComponent<MeshRenderer>().enabled = false;
            jack.GetComponent<Animator>().SetBool("crankConnected", true);
            //lerpCrank();
        }
    }

    IEnumerator lerpCrank()
    {
        while (elapsedTime < desiredDuration)
        {
            crank.transform.position=Vector3.Lerp(startPos.position, crankDest.position, (elapsedTime/desiredDuration));
            crank.transform.rotation=Quaternion.Lerp(startPos.rotation, crankDest.rotation, (elapsedTime/desiredDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("schmoovin");
        yield return null;
    }


}
