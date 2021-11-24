using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Wrapper object for maintaining button references
public class ButtonHandler : MonoBehaviour
{
    public SceneLoader sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        if (sceneManager == null) sceneManager = FindObjectOfType<SceneLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneManager == null) sceneManager = FindObjectOfType<SceneLoader>();
    }

    public void setAssisted(bool b)
    {
        if (sceneManager != null) sceneManager.setAssisted(b);
    }
    
    public void loadScene(string sceneName)
    {
        if (sceneManager != null) sceneManager.loadScene(sceneName);
    }

    public void loadSceneRetry(string sceneName)
    {
        if (sceneManager != null) sceneManager.loadSceneRetry(sceneName);
    }

    public void tireChange(string sceneName)
    {
        if (sceneManager != null) sceneManager.tireChange(sceneName);
    }

    public void batteryJump(string sceneName)
    {
        if (sceneManager != null) sceneManager.batteryJump(sceneName);
    }

    public void pitCrew(string sceneName)
    {
        if (sceneManager != null) sceneManager.pitCrew(sceneName);
    }
}
