using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    private const string _LoadingSceneName = "Loading";

    private static int _gameMode = 0;  // 1 = start, 2 = load, else = 0

    public static int GetGameMode()
    {
        return _gameMode;
    }

    public void ClickToStartGame()
    {
        Debug.Log("Start Button Clicked.");
        _gameMode = 1;
        SceneManager.LoadScene(_LoadingSceneName);
    }

    public void ClickToLoadGame()
    {
        Debug.Log("Load Button Clicked.");
        _gameMode = 2;
        SceneManager.LoadScene(_LoadingSceneName);
    }

    public void ClickToExitGame()
    {
        Debug.Log("Exit Button Clicked.");
        Application.Quit();
    }

}
