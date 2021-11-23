using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Transform : MonoBehaviour
{
    public GameObject parentObject;
    public Vector3 normalAxis;
    public float distance;

    GameObject transformDummy;

    // Start is called before the first frame update
    void Start()
    {

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
                transformDummy = new GameObject("Transform Coordinates");
                transformDummy.transform.SetParent(parentObject.transform);
            }

            transformDummy.transform.localPosition = Vector3.Scale(normalAxis.normalized,InverseVector(parentObject.transform.localScale)) * distance;
            this.gameObject.transform.position = transformDummy.transform.position;
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
