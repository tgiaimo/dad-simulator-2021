using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class Pause_Button : MonoBehaviour
{
    public bool m_ButtonTest;
    public SteamVR_Input_Sources m_TargetSource;
    public SteamVR_Action_Boolean m_TargetAction;
    public GameObject m_PauseMenu, m_PointerHand, m_Pointer;
    public Canvas[] menus;

    GameObject newPointer;
    public bool buttonPressed;
    
    // Start is called before the first frame update
    void Start()
    {
        m_PauseMenu.SetActive(false);
        buttonPressed = false;
        m_PointerHand=GameObject.Find("RightHand");

        /*
        GameObject whatever = GameObject.Find("PR_Pointer");

        foreach (Canvas d in menus)
        {
            d.worldCamera = whatever.GetComponent(typeof(Camera)) as Camera;
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        //Open or close pause menu for button press
        if (m_TargetAction.GetState(m_TargetSource) || m_ButtonTest)
        {
            if (!buttonPressed)
            {
                if (m_PauseMenu.activeSelf)
                {
                    m_PauseMenu.SetActive(false);
                    //removepointer();
                }
                else
                {
                    m_PauseMenu.SetActive(true);
                    //addPointer();
                }
                buttonPressed = true;
            }
        }
        else buttonPressed = false;
    }

    //Adds pointer for menu interaction
    void addPointer()
    {
        newPointer = Instantiate(m_Pointer);
        newPointer.transform.SetParent(m_PointerHand.transform);
        newPointer.transform.localRotation=Quaternion.Euler(0f,0f,0f);
        newPointer.transform.localPosition=new Vector3(0f,0f,0f);
        newPointer.transform.localScale = new Vector3(1f, 1f, 1f);
        
        //Set event camera to created pointer
        foreach (Canvas d in menus)
        {
            d.worldCamera = newPointer.GetComponent(typeof(Camera)) as Camera;
        }
    }

    //Destroys created pointer
    public void removePointer()
    {
        if (newPointer != null)
        {
            GameObject destroyPtr = newPointer;
            Destroy(destroyPtr);
        }

    }

}
