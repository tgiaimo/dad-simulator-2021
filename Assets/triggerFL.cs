using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class triggerFL : MonoBehaviour
{
    public GameObject flat;
    public GameObject good;
    private GameObject tireToLerp;
    public GameObject rightHand;
    public GameObject leftHand;
    public Transform endPos;
    public Transform startPos;
    public float sped;
    public float desiredDuration;
    private float elapsedTime;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == good.name)
        {
            tireToLerp = collider.gameObject;
            startPos = tireToLerp.transform;
            elapsedTime = 0;
            if (rightHand.GetComponent<Hand>().ObjectIsAttached(tireToLerp))
            {
                rightHand.GetComponent<Hand>().DetachObject(tireToLerp);
                collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            if (leftHand.GetComponent<Hand>().ObjectIsAttached(tireToLerp))
            {
                leftHand.GetComponent<Hand>().DetachObject(tireToLerp);
                collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            tireToLerp.transform.eulerAngles = new Vector3((float)1.71386409,(float)351.135559,(float)18.2478504);
            Destroy(tireToLerp.GetComponent<Throwable>());
            Destroy(tireToLerp.GetComponent<Interactable>());
            Destroy(tireToLerp.GetComponent<Rigidbody>());
            tireToLerp.GetComponent<SphereCollider>().enabled = false;
            StartCoroutine(moveTire());
        }
    }
    void OnTriggerStay(Collider collider)
    {
    }

    void OnTriggerExit(Collider collider)
    {
        if(collider.gameObject.name == flat.name)
        {
            collider.gameObject.GetComponent<badTireCheck>().off = true;
        }
    }

    IEnumerator moveTire()
    {
        while (elapsedTime < desiredDuration)
        {
            tireToLerp.transform.position=Vector3.Lerp(startPos.position, endPos.position, (elapsedTime/desiredDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("schmoovin");
        yield return null;
    }



}
