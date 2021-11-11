using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class Pause_Button : MonoBehaviour
{
    public SteamVR_Input_Sources m_TargetSource;
    public SteamVR_Action_Boolean m_TargetAction;
    public GameObject m_PauseMenu, m_PointerHand, m_Pointer;
    public Canvas[] menus;

    GameObject newPointer;
    bool buttonPressed;
    float elapsed_time;
    
    // Start is called before the first frame update
    void Start()
    {
        m_PauseMenu.SetActive(false);
        buttonPressed = false;
        elapsed_time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonPressed)
        {
            elapsed_time += Time.deltaTime;
            if (elapsed_time >= 1) buttonPressed = false;
        }
        
        if (m_TargetAction.GetState(m_TargetSource))
        {
            if (m_PauseMenu.activeSelf)
            {
                m_PauseMenu.SetActive(false);
                removePointer();
            }
            else 
            {
                m_PauseMenu.SetActive(true);
                addPointer();
            }

            buttonPressed = true;
            elapsed_time = 0;
        }
    }

    void addPointer()
    {
        newPointer = Instantiate(m_Pointer);
        newPointer.transform.SetParent(m_PointerHand.transform);
        
        foreach (Canvas d in menus)
        {
            d.worldCamera = GameObject.Find("PR_Pointer").GetComponent(typeof(Camera)) as Camera;
        }
    }

    void removePointer()
    {
        GameObject destroyPtr = newPointer;
        Destroy(destroyPtr);
    }
}
