using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnStart()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    //make a function to quit
    public void OnQuit()
    {
        Application.Quit();
    }
}
