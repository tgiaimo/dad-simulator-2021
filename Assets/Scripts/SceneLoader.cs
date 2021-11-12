using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public float bestTime = 1e9f;
    public bool tire, battery, pitcrew;
    public testing gameContainer;
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

    private void Start()
    {
        elapsed_time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
        /*
        //Check if simulation is running
        if (gameContainer != null && !isComplete)
        {
            //Check for completion depending on game type
            if (tire && !pitcrew)
            {
                if (gameContainer.findStepTire() == 9) isComplete = true;
            }
            else if (battery && !pitcrew)
            {
                if (gameContainer.findStepBat() == 9) isComplete = true;
            }
        }

        //Start timer for game
        if (isComplete)
        {
            elapsed_time += Time.deltaTime;
        }

        //Load menu scene after some time
        if (elapsed_time >= 3)
        {
            loadScene("Environment_Test_Menu");
            isComplete = false;
        }
        */
    }

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
        gameContainer = FindObjectOfType<testing>();
    }

    public void batteryJump(string sceneName)
    {
        tire = false;
        battery = true;
        pitcrew = false;
        loadScene(sceneName);
        gameContainer = FindObjectOfType<testing>();
    }

    public void pitCrew(string sceneName)
    {
        tire = true;
        battery = false;
        pitcrew = true;
        loadScene(sceneName);
        gameContainer = FindObjectOfType<testing>();
    }

    public bool isTire() { return tire; }

    public bool isBattery() { return battery; }
}
