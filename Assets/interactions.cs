using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class interactions : MonoBehaviour
{
    public bool used;
    public GameObject scripts;
    public GameObject yurt;
    public GameObject trigger;
    public GameObject lug;
    public int step;
    public Transform safeSpace;
    public float elapsedTime;
    private float desiredDuration = 0.5f;

    public MeshCollider collider1;
    public MeshCollider collider2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        step = scripts.GetComponent<testing>().findStepTire();
    }

    public void loosen()
    {
        lug.gameObject.transform.GetChild(0).GetComponent<HubCheck>().loosened = true;
        lug.GetComponent<MeshCollider>().enabled = true;
        lug.GetComponent<Interactable>().enabled = true;
        lug.GetComponent<Rigidbody>().isKinematic = false;
        lug.AddComponent<Throwable>();
        makeThrow();
        forceKinematicMove();
    }

    public void actionOnMin()
    {
        if (step == 9)
        {
            Tighten();
        }
    }
    public void actionOnMax()
    {
        if (step == 1)
        {
            unTighten();
        }
        if (step == 4)
        {
            loosen();
        }
    }
    public void Tighten()
    {
        lug.gameObject.transform.GetChild(0).GetComponent<HubCheck>().tight= true;
        makeThrow();
        forceKinematicMove();

    }

    public void unTighten()
    {
        Debug.Log("unTighten");
        lug.gameObject.transform.GetChild(0).GetComponent<HubCheck>().tight= false;
        makeThrow();
        forceKinematicMove();
    }
    public void makeThrow()
    {
        trigger.GetComponent<BoxCollider>().enabled = false;
        trigger.GetComponent<MeshRenderer>().enabled = false;
        //yurt.GetComponent<CircularDrive>().enabled = false;
        //yurt.AddComponent<Throwable>();
        //yurt.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void forceKinematic()
    {
        yurt.GetComponent<Rigidbody>().isKinematic = false;
    }
 
    public void forceKinematicMove()
    {
        StartCoroutine(MoveWrenchSafe());
    }
    
    IEnumerator MoveWrenchSafe()
    {
        while (elapsedTime < desiredDuration)
        {
            yurt.transform.position=Vector3.Lerp(yurt.transform.position, safeSpace.position, (elapsedTime/desiredDuration));
            yurt.transform.rotation=Quaternion.Lerp(yurt.transform.rotation, safeSpace.rotation, (elapsedTime/desiredDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yurt.GetComponent<interactions>().used = false;
        yurt.GetComponent<Rigidbody>().isKinematic = false;
        yurt.GetComponent<Interactable>().enabled = true;
        collider1.enabled = true;
        collider2.enabled = true;
        Debug.Log("schmoovin");
        yield return null;
    }

}
