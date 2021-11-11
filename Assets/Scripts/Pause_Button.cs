using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Pause_Button : MonoBehaviour
{
    public SteamVR_Input_Sources m_TargetSource;
    public SteamVR_Action_Boolean m_TargetAction;
    public GameObject m_PauseMenu, m_Pointer;

    bool buttonPressed;
    float elapsed_time;
    
    // Start is called before the first frame update
    void Start()
    {
        m_PauseMenu.SetActive(false);
        m_Pointer.SetActive(false);
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
            if (m_PauseMenu.activeInHierarchy)
            {
                m_PauseMenu.SetActive(false);
                m_PauseMenu.SetActive(true);
            }
            else m_PauseMenu.SetActive(true);

            buttonPressed = true;
            elapsed_time = 0;
        }
    }
}
