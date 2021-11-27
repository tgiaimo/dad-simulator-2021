using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class triggerFL : MonoBehaviour
{
    public Transform car;
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
            elapsedTime = 0;
            if (rightHand.GetComponent<Hand>().ObjectIsAttached(tireToLerp))
            {
                rightHand.GetComponent<Hand>().DetachObject(tireToLerp);
                collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                tireToLerp.transform.SetParent(car);
                startPos = tireToLerp.transform;
            }
            if (leftHand.GetComponent<Hand>().ObjectIsAttached(tireToLerp))
            {
                leftHand.GetComponent<Hand>().DetachObject(tireToLerp);
                collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                tireToLerp.transform.SetParent(car);
                startPos = tireToLerp.transform;
            }
            
            Destroy(tireToLerp.GetComponent<Throwable>());
            //tireToLerp.transform.eulerAngles = new Vector3((float)1.71386409,(float)351.135559,(float)18.2478504);
            tireToLerp.GetComponent<SphereCollider>().enabled = false;
            StartCoroutine(moveTire());
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            collider.gameObject.GetComponent<goodTireCheck>().on = true;

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
            tireToLerp.transform.rotation=Quaternion.Lerp(startPos.rotation, endPos.rotation, (elapsedTime/desiredDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("schmoovin");
        yield return null;
    }



}
