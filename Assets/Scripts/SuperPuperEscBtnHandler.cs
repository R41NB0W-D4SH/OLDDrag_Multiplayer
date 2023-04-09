using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SuperPuperEscBtnHandler : MonoBehaviour
{
    [SerializeField] private int _scene_id;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.UnloadSceneAsync(_scene_id); 
        } 
    }
}