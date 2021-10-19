using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_script : MonoBehaviour
{
    GameObject mainMenu, tireMenu, jumpMenu;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu = GameObject.Find("Main Menu");
        tireMenu = GameObject.Find("Tire Change Submenu");
        jumpMenu = GameObject.Find("Battery Jump Submenu");

        mainMenu.SetActive(true);
        tireMenu.SetActive(false);
        jumpMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void tireChangePressed()
    {
        mainMenu.SetActive(false);
        tireMenu.SetActive(true);
    }

    public void tireChangeBack()
    {
        tireMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void batteryJumpPressed()
    {
        mainMenu.SetActive(false);
        jumpMenu.SetActive(true);
    }

    public void batteryJumpBack()
    {
        jumpMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void exitToDesktop()
    {
        Application.Quit();
        Debug.Log("Exiting to Desktop");
    }

    public void buttonTest()
    {
        Debug.Log("Button Pressed");
    }

}