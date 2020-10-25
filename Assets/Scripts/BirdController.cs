using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BirdController : MonoBehaviour
{
    private const float _TimeOfReLoadCurrentScene = 3.5f;

    private static float _initialAttackValue = 25f;

    private bool _wasLaunched;
    private float _timeCount;
    private float _gameWidthBoundary;
    private float _gameHeightBoundary;
    private float _birdMoveWidthBoundary;
    private float _birdMoveHeightBoundary;
    private Vector3 _initialPosition;
    private float _currentAttackValue;

    [SerializeField]
    private float _launchSpeed = 500;
    [SerializeField]
    private float _gravity = 10;


    public static void SetInitialAttackValue(float setValue)
    {
        _initialAttackValue = setValue;
    }

    public static float GetInitialAttackValue()
    {
        return _initialAttackValue;
    }

    public void SetCurrentAttackPowerUp()
    {
        _currentAttackValue += _currentAttackValue;
        Debug.Log("BirdController: Bird Current Attack(PowerUp) = " + _currentAttackValue);
    }

    public void SetCurrentAttackValue(float setValue)
    {
        _currentAttackValue = setValue;
    }

    public float GetCurrentAttackValue()
    {
        return _currentAttackValue;
    }


    void Awake()
    {
        //Debug.Log("Screen Width = " + Screen.width + " , " + "Screen Height = " + Screen.height);
        _wasLaunched = false;
        _timeCount = 0f;

        _birdMoveWidthBoundary = (float)25.1;
        _birdMoveHeightBoundary = (float)9.4;
        //Debug.Log("Bird Width Limit = " + _birdMoveWidthBoundary + " , " + "Bird Height Limit = " + _birdMoveHeightBoundary);

        _gameWidthBoundary = _birdMoveWidthBoundary + 1;
        _gameHeightBoundary = _birdMoveHeightBoundary + 1;
        //Debug.Log("Game Width Limit = " + _gameWidthBoundary + " , " + "Game Height Limit= " + _gameHeightBoundary);

        _initialPosition = transform.position;
        Debug.Log("BirdController.Awake() Bird Current Attack = " + _currentAttackValue);

        Debug.Log("BirdController.Awake() is done.");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameProcessController._wasExitDialogBoxShown)
        {
            transform.position = GameProcessController.GetCurrentBirdPositionPaused();
            //GetComponent<Rigidbody2D>().gravityScale = _gravity / 10;
            GetComponent<Rigidbody2D>().velocity = GameProcessController.GetCurrentBirdSpeedPaused();
            return;
        }

        Vector2 currentBirdSpeed = GetComponent<Rigidbody2D>().velocity;
        if (_wasLaunched && currentBirdSpeed.magnitude <= 0.1f) 
        {
            _timeCount += Time.deltaTime;
        }

        if (_timeCount > _TimeOfReLoadCurrentScene ||
            transform.position.x > _gameWidthBoundary ||
            transform.position.y > _gameHeightBoundary || transform.position.y < -_gameHeightBoundary)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }

    void OnMouseDown()
    {
        if (GameProcessController._wasExitDialogBoxShown || _wasLaunched)
        {
            return;
        }

        GetComponent<SpriteRenderer>().color = Color.red;
        //Debug.Log("BirdController.OnMouseDown() is done.");
    }

    void OnMouseUp()
    {
        if (GameProcessController._wasExitDialogBoxShown || _wasLaunched)
        {
            return;
        }

        GetComponent<SpriteRenderer>().color = Color.white;

        Vector2 directionToInitialPosition = _initialPosition - transform.position;
        GetComponent<Rigidbody2D>().AddForce(directionToInitialPosition * _launchSpeed);
        GetComponent<Rigidbody2D>().gravityScale = _gravity;
        _wasLaunched = true;

        //Debug.Log("BirdController.OnMouseUp() is done.");
    }

    void OnMouseDrag()
    {
        if (GameProcessController._wasExitDialogBoxShown || _wasLaunched)
        {
            return;
        }

        Vector3 curBirdPosScreenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        curBirdPosScreenToWorld.z = 0;
        if (curBirdPosScreenToWorld.x > _initialPosition.x)
        {
            curBirdPosScreenToWorld.x = _initialPosition.x;
        }
        else if (curBirdPosScreenToWorld.x < -_birdMoveWidthBoundary)
        {
            curBirdPosScreenToWorld.x = -_birdMoveWidthBoundary;
        }

        if (curBirdPosScreenToWorld.y > _birdMoveHeightBoundary)
        {
            curBirdPosScreenToWorld.y = _birdMoveHeightBoundary;
        }
        else if (curBirdPosScreenToWorld.y < -_birdMoveHeightBoundary)
        {
            curBirdPosScreenToWorld.y = -_birdMoveHeightBoundary;
        }

        transform.position = curBirdPosScreenToWorld;
        //Debug.Log("BirdController.OnMouseDrag() is done.");
    }

}
