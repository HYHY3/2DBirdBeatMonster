using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    private const string _GameArchivedFileName = "GameSave.save";
    private const string _GameSceneName = "Level";
    private const int _GameModeOfStart = 1;
    private const int _GameModeOfLoad = 2;

    public static bool _isGameOnGoing = false;

    private static SaveLoadController _gameInfoFromSaved = null; // new SaveLoadController();

    private int _sceneLevel;
    private Slider _loadingBarProgress;
    //private Text _loadingBarValue;


    public static SaveLoadController GetGameInfomationFromSaved()
    {
        return _gameInfoFromSaved;
    }

    private void Awake()
    {
        _loadingBarProgress = GetComponentInChildren<Slider>();
        //_loadingBarValue = GetComponentInChildren<Text>();
        if (_loadingBarProgress == null) // || _loadingBarValue == null)
        {
            Debug.Log("LoadingController.Awake() LoadingBar is null gameobject.");
            return;
        }

        if (_isGameOnGoing)
        {
            return;
        }

        if (GameMenuController.GetGameMode() == _GameModeOfStart)
        {
            _sceneLevel = 1;
            _isGameOnGoing = true;

            Debug.Log("LoadingController.Awake() Game Start Mode is done.");
        }
        else if (GameMenuController.GetGameMode() == _GameModeOfLoad)
        {
            if (!LoadGameInformationFromSaved())
            {
                Debug.Log("LoadingController.Awake() No File to Load!!!");
                return;
            }

            GameProcessController._doesLoadGame = true;
            _sceneLevel = _gameInfoFromSaved._currentSceneLevel;
            _isGameOnGoing = true;

            Debug.Log("LoadingController.Awake() Game Loaded Mode is done");
        }
        else
        {
            _sceneLevel = 0;
            _isGameOnGoing = false;
            Debug.Log("LoadingController.Awake() Game Menu Mode is done.");
        }

        //Debug.Log("LoadingController.Awake() Scene Level = " + _sceneLevel);
        GameProcessController.SetCurrentSceneLevel(_sceneLevel);

        Debug.Log("LoadingController.Awake() is done.");
    }

    private void Start()
    {
        //Debug.Log("LoadingController.Start() Scene Level = " + GameProcessController.GetCurrentSceneLevel());
        LoadNextScene(_GameSceneName + GameProcessController.GetCurrentSceneLevel());

        Debug.Log("LoadingController.Start() is done.");
    }

    private bool LoadGameInformationFromSaved()
    {
        string filePath = Application.persistentDataPath + "/" + _GameArchivedFileName;
        if (!File.Exists(filePath))
        {
            //Debug.Log("LoadingController.LoadGameInformationFromSaved() No File to Load!!!");
            return false;
        }

        BinaryFormatter binaryformat = new BinaryFormatter();
        FileStream file = File.Open(filePath, FileMode.Open);
        _gameInfoFromSaved = (SaveLoadController)binaryformat.Deserialize(file);
        file.Close();

        //Debug.Log("LoadingController.LoadGameInformationFromSaved() Game is Loaded");
        return true;
    }

    private void SetLoadingPercentage(float num)
    {
        _loadingBarProgress.value = num;
        //_loadingBarValue.text = num.ToString() + "%";
    }

    private IEnumerator StartLoading(string sceneName)
    {
        int displayProgress = 0;
        int toProgress = 0;

        //Debug.Log("LoadingController.StartLoading() Scene Name = " + sceneName);
        AsyncOperation asynOp = SceneManager.LoadSceneAsync(sceneName);
        asynOp.allowSceneActivation = false;
        //Debug.Log("LoadingController.StartLoading() asyn Progress = " + asynOp.progress);
        while (asynOp.progress < 0.9f)
        {
            //Debug.Log("LoadingController.StartLoading() asyn Progress = " + asynOp.progress);
            toProgress = (int)(asynOp.progress * 1000);
            //Debug.Log("LoadingController.StartLoading() to Progress = " + toProgress);
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                //Debug.Log("LoadingController.StartLoading() Display Progress = " + displayProgress);
                SetLoadingPercentage(displayProgress);
                yield return new WaitForEndOfFrame();
            }
        }

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            //Debug.Log("LoadingController.StartLoading() Progress = " + displayProgress);
            SetLoadingPercentage(displayProgress);
            yield return new WaitForEndOfFrame();
        }
        asynOp.allowSceneActivation = true;
    }

    private void LoadNextScene(string sceneName)
    {
        StartCoroutine(StartLoading(sceneName));
        Debug.Log("LoadingController.LoadNextScene() is done.");
    }
}
