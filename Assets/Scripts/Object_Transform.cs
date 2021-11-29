using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Transform : MonoBehaviour
{
    public GameObject parentObject;
    public Vector3 normalAxis;
    public Vector3 normalOrientation;
    public float distance;

    GameObject transformDummy;

    // Start is called before the first frame update
    void Start()
    {
        if (parentObject == null)
        {
            parentObject = GameObject.Find("VRCamera");
            if (parentObject == null)
            {
                parentObject = GameObject.Find("FallbackObjects");
            }
        }
    }

    private void OnDestroy()
    {
        Destroy(transformDummy);
    }

    // Update is called once per frame
    void Update()
    {
        if (parentObject != null)
        {
            if (transformDummy == null)
            {
                transformDummy = new GameObject("Transform Coordinates: " + this.gameObject.name);
                transformDummy.transform.SetParent(parentObject.transform);
            }

            transformDummy.transform.localPosition = Vector3.Scale(normalAxis.normalized,InverseVector(parentObject.transform.localScale)) * distance;
            transformDummy.transform.localRotation = Quaternion.Euler(normalOrientation.x, normalOrientation.y, normalOrientation.z);
            this.gameObject.transform.position = transformDummy.transform.position;
            this.gameObject.transform.rotation = transformDummy.transform.rotation;
        }
        
    }

    Vector3 InverseVector(Vector3 input)
    {
        Vector3 output = new Vector3();
        output.x = 1 / input.x;
        output.y = 1 / input.y;
        output.z = 1 / input.z;
        return output;
    }
}
