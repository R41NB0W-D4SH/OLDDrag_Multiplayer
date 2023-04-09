using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
    public void Rl(int sceneid)
    {
        SceneManager.LoadScene(sceneid);
    }
}
