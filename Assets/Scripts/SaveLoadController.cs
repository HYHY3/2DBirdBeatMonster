using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyInformation
{
    //public Vector3 _currentEnemyPosition;
    public string _currentEnemyName;
    public float _currentEnemyLifeValue;
};

[System.Serializable]
public class SaveLoadController
{
    public int _currentSceneLevel;
    public float _currentBirdAttackValue;
    public List<EnemyInformation> _currentEnemyInformation;

    public SaveLoadController()
    {
        _currentEnemyInformation = new List<EnemyInformation>();
    }
}
