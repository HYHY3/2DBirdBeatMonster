using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameProcessController : MonoBehaviour
{
    private const string _GameSceneName = "Level";
    private const string _ExitDialogBoxName = "ExitDialogBoxTag";
    private const string _InvokedFunctionName = "GoToNextScene";
    private const string _GameArchivedFileName = "GameSave.save";
    private const string _LoadingSceneName = "Loading";
    private const int _FirstSceneLevel = 1;
    private const int _LastSceneLevel = 4;
    private const float _TimeDelayToNextScene = 2.5f;
    
    public static bool _wasExitDialogBoxShown = false;
    public static bool _doesLoadGame = false;
    private static bool _wasSceneInitialized = false;
    private static int _currentLevel = 0;
    private static Dictionary<string, float> _currentEnemiesLifeValue;
    private static Vector3 _currentBirdPositionPaused;
    private static Vector2 _currentBirdSpeedPaused;


    private BirdController _bird;
    private EnemyController[] _enemies;
    private bool _wasTimeDelayStarted;
    private GameObject _exitDialogBox;
    private SaveLoadController _gameInfoFromSaved;

    //private int _LastSceneLevel;


    public static Vector3 GetCurrentBirdSpeedPaused()
    {
        return _currentBirdSpeedPaused;
    }

    public static Vector3 GetCurrentBirdPositionPaused()
    {
        return _currentBirdPositionPaused;
    }

    public static int GetCurrentSceneLevel()
    {
        return _currentLevel;
    }

    public static void SetCurrentSceneLevel(int setVal)
    {
        _currentLevel = setVal;
    }

    private void SetGameSceneState()
    {
        _bird.SetCurrentAttackValue(BirdController.GetInitialAttackValue());

        if (_doesLoadGame)
        {
            _doesLoadGame = false;
            _wasSceneInitialized = true;
            _gameInfoFromSaved = LoadingController.GetGameInfomationFromSaved();

            _bird.SetCurrentAttackValue(_gameInfoFromSaved._currentBirdAttackValue);

            _currentEnemiesLifeValue = new Dictionary<string, float>();
            foreach (EnemyInformation enemyInfo in _gameInfoFromSaved._currentEnemyInformation)
            {
                //Debug.Log("GameProcessController.SetGameSceneState " + enemyInfo._currentEnemyName + " = " + enemyInfo._currentEnemyLifeValue);
                _currentEnemiesLifeValue.Add(enemyInfo._currentEnemyName, enemyInfo._currentEnemyLifeValue);
            }
        }
        if (!_wasSceneInitialized)
        {
            _wasSceneInitialized = true;

            _currentEnemiesLifeValue = new Dictionary<string, float>();
            foreach (EnemyController enemy in _enemies)
            {
                _currentEnemiesLifeValue.Add(enemy.name, enemy.GetMaxLifeValue());
            }
        }

        float currentEnemyLife;
        foreach (EnemyController enemy in _enemies)
        {
            if(!_currentEnemiesLifeValue.TryGetValue(enemy.name, out currentEnemyLife))
            {
                Debug.Log("GameProcessController.SetGameSceneState() " + enemy.name + " is not exist.");
                continue;
            }

            enemy.gameObject.SetActive(true);
            if (currentEnemyLife <= 0.4f)
            {
                enemy.gameObject.SetActive(false);
                continue;
            }
            enemy.SetCurrentLifeValue(currentEnemyLife);
            Debug.Log("GameProcessController.SetGameSceneState() " + enemy.name + " Current Life = " + enemy.GetCurrentLifeValue());
        }

        Debug.Log("GameProcessController.SetGameSceneState() Scene Setting is done.");
    }

    void Awake()
    {
        _bird = FindObjectOfType<BirdController>();
        _enemies = FindObjectsOfType<EnemyController>();
        Debug.Log("GameProcessController.Awake() Enemy Count = " + _enemies.Length);
        
        //_LastSceneLevel = SceneManager.sceneCount;
        //Debug.Log(_LastSceneLevel);
        _wasTimeDelayStarted = false;
        _wasExitDialogBoxShown = false;

        _exitDialogBox = GameObject.FindWithTag(_ExitDialogBoxName);
        if (_exitDialogBox == null)
        {
            Debug.Log("GameProcessController.Awake() ExitDialogBox is null gameobject.");
            return;
        }
        _exitDialogBox.SetActive(false);

        SetGameSceneState();

        Debug.Log("GameProcessController.Awake() is done.");
    }

    void Update()
    {
        if (_wasExitDialogBoxShown)    // ExitDialogBox was shown
        {
            if (IsInvoking(_InvokedFunctionName))
            {
                _wasSceneInitialized = false;
                CancelInvoke(_InvokedFunctionName);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))    // Esc key was pressed
        {
            ResponseExitDialogBoxProcess();
            return;
        }
        else if (Input.GetKeyDown(KeyCode.A))    // A key was pressed
        {
            _bird.SetCurrentAttackPowerUp();
        }

        if (_wasTimeDelayStarted)
        {
            //Debug.Log("curTime_2 = :" + Time.time);
            return;
        }

        bool isAnyEnemyAlive = false;
        foreach (EnemyController enemy in _enemies)
        {
            float currentEnemyLife = enemy.GetCurrentLifeValue();
            if (currentEnemyLife >= 0.5f)
            {
                isAnyEnemyAlive = true;
            }
            _currentEnemiesLifeValue[enemy.name] = enemy.GetCurrentLifeValue();
        }
        if (isAnyEnemyAlive)
        {
            return;
        }
        //Debug.Log("curTime_1 = :" + Time.time);
        Invoke(_InvokedFunctionName, _TimeDelayToNextScene);
        _wasTimeDelayStarted = true;
    }

    private void GoToNextScene()
    {
        ++_currentLevel;
        if (_currentLevel == _LastSceneLevel)
        {
            _currentLevel = _FirstSceneLevel;
        }
        //Debug.Log("GameProcessController.GoToNextScene() nextlevel = " + _currentLevel);
        _wasSceneInitialized = false;
        SceneManager.LoadScene(_LoadingSceneName);
        //SceneManager.LoadScene(_GameSceneName + _currentLevel);
    }

    private void ResponseExitDialogBoxProcess()
    {
        _wasExitDialogBoxShown = true;
        _exitDialogBox.SetActive(true);
        _currentBirdPositionPaused = _bird.transform.position;
        _currentBirdSpeedPaused = _bird.GetComponent<Rigidbody2D>().velocity;
    }

    public void OnSaveButtonClick()
    {
        _exitDialogBox.SetActive(false);
        _wasExitDialogBoxShown = false;
        _wasSceneInitialized = false;
        //save
        SaveLoadController saveInfo = CreateSaveGameObject();
        FileStream file = File.Create(Application.persistentDataPath + "/" + _GameArchivedFileName);
        BinaryFormatter binaryFormat = new BinaryFormatter();
        binaryFormat.Serialize(file, saveInfo);
        file.Close();

        Debug.Log("GameProcessController.OnSaveButtonClick() SavePath = " + Application.persistentDataPath);
        Debug.Log("GameProcessController.OnSaveButtonClick() Save successed.");
        LoadingController._isGameOnGoing = false;
        _currentLevel = 0;
        SceneManager.LoadScene(_GameSceneName + _currentLevel);
    }

    public void OnNoSaveButtonClick()
    {
        _exitDialogBox.SetActive(false);
        _wasExitDialogBoxShown = false;
        _wasSceneInitialized = false;
        LoadingController._isGameOnGoing = false;
        _currentLevel = 0;
        SceneManager.LoadScene(_GameSceneName + _currentLevel);
    }

    public void OnCancelButtonClick()
    {
        _exitDialogBox.SetActive(false);
        _wasExitDialogBoxShown = false;
    }

    private SaveLoadController CreateSaveGameObject()
    {
        SaveLoadController save = new SaveLoadController();

        save._currentSceneLevel = _currentLevel;
        save._currentBirdAttackValue = _bird.GetCurrentAttackValue();

        foreach (EnemyController enemy in _enemies)
        {
            EnemyInformation enemyInfo = new EnemyInformation();
            //enemyInfo._currentEnemyPosition = enemy.transform.position;
            enemyInfo._currentEnemyName = enemy.name;
            enemyInfo._currentEnemyLifeValue = enemy.GetCurrentLifeValue();
            //Debug.Log("GameProcessController.CreateSaveGameObject() " + enemyInfo._currentEnemyName + " = " + enemyInfo._currentEnemyLifeValue);
            save._currentEnemyInformation.Add(enemyInfo);
        }
        return save;
    }

}
