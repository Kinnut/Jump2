using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class _SceneManager : MonoBehaviour
{
    public void ToStartScene()
    {
        SceneManager.LoadScene("0.StartScene");
    }

    public void ToPlayScene()
    {
        SceneManager.LoadScene("1.PlayScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
