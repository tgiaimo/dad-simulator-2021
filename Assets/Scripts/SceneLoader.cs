using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public float bestTime = 1e9f;
    public bool tire, battery, assisted, pitcrew;
    public testing gameContainer;
    public Canvas exitScreen;
    public Text timerText;
    bool isComplete = false;
    float elapsed_time;

    // Start is called before the first frame update
    void Awake()
    {
        int destroyableObjects = FindObjectsOfType<SceneLoader>().Length;

        if (destroyableObjects != 1)
        {
            Destroy(this.gameObject);
        }

        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        exitScreen.gameObject.SetActive(false);
        elapsed_time = 5f;
    }


    // Update is called once per frame
    void Update()
    {
        //Check if simulation is running
        if (gameContainer != null && !isComplete)
        {
            //Check for completion depending on game type
            if (tire && !pitcrew)
            {
                if (gameContainer.tireComplete) isComplete = true;
            }
            else if (battery && !pitcrew)
            {
                if (gameContainer.batComplete) isComplete = true;
            }
        }

        //Start timer for game
        if (isComplete)
        {
            exitScreen.gameObject.SetActive(true);
            elapsed_time -= Time.deltaTime;
            timerText.text = "Simulation Complete: \nExiting in: " + formatTime(elapsed_time);
        }

        //Load menu scene after some time
        if (elapsed_time <= 0)
        {
            exitScreen.gameObject.SetActive(false);
            loadScene("Environment_Test_Title");
            isComplete = false;
            elapsed_time = 5f;
        }
    }

    public void setAssisted(bool b) { assisted = b; }

    public void loadScene(string sceneName)
    {
        Destroy(GameObject.Find("Player"));
        SceneManager.LoadScene(sceneName);
    }

    public void loadSceneRetry(string sceneName)
    {
        Destroy(GameObject.Find("Player"));
        SceneManager.LoadScene(sceneName);
    }

    public void tireChange(string sceneName)
    {
        tire = true;
        battery = false;
        pitcrew = false;
        loadScene(sceneName);
    }

    public void batteryJump(string sceneName)
    {
        tire = false;
        battery = true;
        pitcrew = false;
        loadScene(sceneName);
    }

    public void pitCrew(string sceneName)
    {
        tire = true;
        battery = false;
        pitcrew = true;
        assisted = false;
        loadScene(sceneName);
    }

    public void setSimScript(testing c) { gameContainer = c;  }

    public bool isTire() { return tire; }

    public bool isBattery() { return battery; }

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
}
