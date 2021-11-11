using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public float bestTime = 1e9f;
    public bool tire = true, battery = false;

    // Start is called before the first frame update
    void Awake()
    {
        int destroyableObjects = FindObjectsOfType<SceneLoader>().Length;

        if (destroyableObjects > 1)
        {
            Destroy(this.gameObject);
        }
        else
            DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadScene(string sceneName)
    {
        Destroy(GameObject.Find("Player"));
        SceneManager.LoadScene(sceneName);
    }

    public void loadSceneRetry(string sceneName)
    {
        Destroy(GameObject.Find("Player"));
        SceneManager.UnloadSceneAsync(sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public void tireChange(string sceneName)
    {
        tire = true;
        battery = false;
        loadScene(sceneName);
    }

    public void batteryJump(string sceneName)
    {
        tire = false;
        battery = true;
        loadScene(sceneName);
    }

    public bool isTire() { return tire; }

    public bool isBattery() { return battery; }
}
