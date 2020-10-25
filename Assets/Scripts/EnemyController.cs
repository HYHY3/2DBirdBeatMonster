using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    private const float _AttackedValueByObstacle = 10f;

    private bool _wasEnemyHit;
    private float _hitAngleLimit;
    private Slider _healthBar;
    private float _maxLifeValue;
    private float _currentLifeValue;

    [SerializeField]
    private GameObject _cloudParticlePrefab = null;


    public float GetMaxLifeValue()
    {
        return _maxLifeValue;
    }

    public float GetCurrentLifeValue()
    {
        return _currentLifeValue;
    }

    public void SetCurrentLifeValue(float setValue)
    {
        _currentLifeValue = setValue;
    }

    private void Awake()
    {
        _wasEnemyHit = false;
        _hitAngleLimit = -0.5f;

        _healthBar = GetComponentInChildren<Slider>();
        if (_healthBar == null)
        {
            Debug.Log("EnemyController.Awake() HealthBar is null gameobject.");
            return;
        }
        _maxLifeValue = _healthBar.maxValue;
        //Debug.Log("EnemyController.Awake() " + gameObject.name + " Max Life = " + _maxLifeValue);

        Debug.Log("EnemyController.Awake() " + gameObject.name + " is done.");
    }

    private void Update()
    {
        if (GameProcessController._wasExitDialogBoxShown)
        {
            return;
        }

        _healthBar.value = _currentLifeValue;
        _healthBar.transform.position = new Vector3(transform.position.x, transform.position.y + transform.localScale.y * (float)5, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameProcessController._wasExitDialogBoxShown)
        {
            return;
        }
        Debug.Log("EnemyController.OnCollisionEnter2D() " + gameObject.name + " Current Life = " + _currentLifeValue);
        //Debug.Log("Normal.(X,Y) = " + collision.contacts[0].normal.x + ", " + collision.contacts[0].normal.y);

        BirdController bird = collision.collider.GetComponent<BirdController>();
        if (!_wasEnemyHit && bird != null)
        {
            _wasEnemyHit = true;
            _currentLifeValue -= bird.GetCurrentAttackValue();
            Debug.Log("EnemyController.OnCollisionEnter2D() " + gameObject.name + " AfterBirdHit = " + _currentLifeValue);
            if (_currentLifeValue <= 0.1f)
            {
                Instantiate(_cloudParticlePrefab, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
            return;
        }

        EnemyController enemy = collision.collider.GetComponent<EnemyController>();
        if (enemy != null)
        {
            return;
        }

        if (!_wasEnemyHit && collision.contacts[0].normal.y < _hitAngleLimit)
        {
            _wasEnemyHit = true;
            _currentLifeValue -= _AttackedValueByObstacle;
            Debug.Log("EnemyController.OnCollisionEnter2D() " + gameObject.name + " AfterBoxHit = " + _currentLifeValue);
            if (_currentLifeValue <= 0.1f)
            {
                Instantiate(_cloudParticlePrefab, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
            return;
        }
    }

}
