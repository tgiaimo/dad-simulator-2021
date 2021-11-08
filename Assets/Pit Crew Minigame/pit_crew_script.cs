using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pit_crew_script : MonoBehaviour
{
    public Text timer;
    public Text sectionTimer;
    public testing steps;

    int numSteps = 10, current_step = 1;
    float elapsed_time, best_time = 1e9f, final_time;
    bool gameEnded = false;
    bool[] step_completed;
    float[] step_times;
    string[] step_names;
    
    // Start is called before the first frame update
    void Start()
    {
        elapsed_time = 0f;
        timer.text = formatTime(elapsed_time);
        sectionTimer.text = "Last Step: " + formatTime(elapsed_time);
        step_completed = new bool[numSteps];
        step_times = new float[numSteps];
        step_names = new string[]
        {
            "Safe Location: ", "Loosen Lug Nuts: ", "Jack Placement: ", "Raise Jack: ", "Undo Lug Nuts: ",
            "Remove Bad Tire: ", "Mount Spare Tire: ", "Tighten Lug Nuts By Hand: ", "Lower Vehicle: ",
            "Tighten Lug Nuts With Wrench: "
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (gameEnded)
        {
            final_time = elapsed_time;
            if (final_time < best_time) best_time = final_time;

            timer.text = "Final Time:  " + formatTime(final_time) + "\n" + "Best Time:  " + formatTime(best_time);

            sectionTimer.text = "";
            for (int i=0; i<numSteps; i++)
            {
                sectionTimer.text += step_names[i] + formatTime(step_times[i]) + "\n";
            }
        }
        else
        {
            //Update Main Timer
            elapsed_time += Time.deltaTime;
            timer.text = "Time:\t" + formatTime(elapsed_time);

            //Update Section Timer
            if (checkStep())
            {
                sectionTimer.text = step_names[current_step - 2] + formatTime(step_times[current_step - 2]);
            }

            gameEnded = checkForEnding();
        }

    }


    //Checks current step for completion, returns true if step is completed and advances step counter
    bool checkStep()
    {
        bool stepCheck = false;
        switch (current_step){ // Check appropriate step value
            case 1:
                stepCheck = steps.tirestep1;
                break;
            case 2:
                stepCheck = steps.tirestep2;
                break;
            case 3:
                stepCheck = steps.tirestep3;
                break;
            case 4:
                stepCheck = steps.tirestep4;
                break;
            case 5:
                stepCheck = steps.tirestep5;
                break;
            case 6:
                stepCheck = steps.tirestep6;
                break;
            case 7:
                stepCheck = steps.tirestep7;
                break;
            case 8:
                stepCheck = steps.tirestep8;
                break;
            case 9:
                stepCheck = steps.tirestep9;
                break;
            case 10:
                stepCheck = steps.tirestep10;
                break;
        }

        //Update section time array upon step completion
        if (stepCheck)
        {
            step_completed[current_step - 1] = true;
            step_times[current_step - 1] = elapsed_time;
            current_step++;
        }

        return stepCheck;
    }

    //Returns a formatted time string from a float value
    string formatTime(float timeDecimal)
    {
        string time_out = "";
        int hours = 0, minutes = 0;
        double seconds;

        //Add hours if time > 1 hr
        while (timeDecimal >= 3600f)
        {
            timeDecimal -= 3600f;
            hours++;
        }

        //Add minutes if time > 1 min
        while (timeDecimal >= 60f)
        {
            timeDecimal -= 60f;
            minutes++;
        }

        //Round seconds to 2 decimal places
        seconds = System.Math.Round(timeDecimal, 2);

        //Format time > 1 hr
        if (hours > 0)
        {
            time_out += hours.ToString() + ":";
            if (minutes < 10) time_out += "0" + minutes.ToString() + ":";
            else time_out += minutes.ToString() + ":";
        }
        //Format time >1 min and <1 hr
        else if (minutes > 0)
        {
            time_out += minutes.ToString() + ":";
        }

        //Format seconds
        if (seconds < 10 && (minutes > 0 || hours > 0)) time_out += "0" + seconds.ToString();
        else time_out += seconds.ToString();

        return time_out;
    }


    //checks for game ending
    bool checkForEnding()
    {
        foreach (bool i in step_completed)
        {
            if (!i) return false;
        }

        return true;
    }
}
