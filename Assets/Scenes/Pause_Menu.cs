using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause_Menu : MonoBehaviour
{
    public GameObject PauseMenuContainer, PauseMenu, ExitConfirm, TaskList;
    public testing simScript;
    public Text taskText;
    public GameObject[] check;
    public Pause_Button Button;

    string[] taskList_tire, taskList_battery;
    
    // Start is called before the first frame update
    void Start()
    {
        PauseMenu.SetActive(true);
        ExitConfirm.SetActive(false);
        TaskList.SetActive(false);

        taskList_tire = new string[]
        {
            "Safe Location", "Loosen Lug Nuts", "Jack Placement", "Raise Jack", "Undo Lug Nuts",
            "Remove Bad Tire", "Mount Spare Tire", "Tighten Lug Nuts By Hand", "Lower Vehicle",
            "Tighten Lug Nuts With Wrench"
        };

        taskList_battery = new string[]
        {
            "Safe Location", "Pop Hood", "Red to Positive of Dead", "Red to Positive of Good", "Ground to Negative of Good",
            "Ground to Negative of Dead", "Crank Working then Dead Car", "Remove Ground to Negative of Dead", 
            "Remove Ground to Negative of Good", "Remove Red to Positive of Good", "Remove Red to Positive of Dead"
        };
    }

    private void OnEnable()
    {
        PauseMenu.SetActive(true);
        ExitConfirm.SetActive(false);
        TaskList.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Create the task list when button is pressed
    public void taskListPressed()
    {
        PauseMenu.SetActive(false);
        TaskList.SetActive(true);
        int stepNumber = 0;

        foreach (GameObject obj in check) obj.SetActive(false);

        taskText.text = "";
        if (simScript.tire)
        {
            stepNumber = simScript.findStepTire();
            foreach (string d in taskList_tire)
            {
                taskText.text += d + "\n";
            }
        }
        else if (simScript.battery)
        {
            stepNumber = simScript.findStepBat();
            foreach (string d in taskList_tire)
            {
                taskText.text += d + "\n";
            }
        }

        for (int i = 0; i <= stepNumber; i++) check[i].SetActive(true);
    }

    //Handle back button pressed
    public void backButtonPressed()
    {
        TaskList.SetActive(false);
        PauseMenu.SetActive(true);
    }

    //Handle "no" on exit to menu
    public void notConfirmedButtonPressed()
    {
        ExitConfirm.SetActive(false);
        PauseMenu.SetActive(true);
    }

    //Handle "exit" button press
    public void exitButtonPressed()
    {
        ExitConfirm.SetActive(true);
        PauseMenu.SetActive(false);
    }

    //Handle "resume"
    public void resumeButtonPressed()
    {
        PauseMenuContainer.SetActive(false);
        Button.removePointer();
    }

}
