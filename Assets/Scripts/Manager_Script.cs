using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager_Script : MonoBehaviour
{
    public void SceneOpen(int sceneid) 
    {
        SceneManager.LoadScene(sceneid, LoadSceneMode.Additive);
    }

    public void SceneExit(int sceneid) 
    {
        SceneManager.UnloadSceneAsync(sceneid);
    }

    public void SceneOpen_Single(int sceneid) 
    {
        SceneManager.LoadScene(sceneid);
    }

    public void QuitTheGame()
    {
        Application.Quit();
    }
}
