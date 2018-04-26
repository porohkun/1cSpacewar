using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Game : MonoBehaviour
{
    static readonly Vector2 HorizontalMirror = new Vector2(-1f, 1f);
    static readonly Vector2 VerticalMirror = new Vector2(1f, -1f);

    [SerializeField]
    private float _speed = 1f;
    [SerializeField]
    private float _accelerationMod = 2f;
    [SerializeField]
    private float _accelerationPow = 1f;
    [SerializeField]
    private float _rotateSpeed = 1f;
    [SerializeField]
    private float _accelerationTime = 0.5f;
    [SerializeField]
    private float _ricochetTime = 0.3f;
    [SerializeField]
    private CameraHandler _camera;
    [SerializeField]
    private Transform _ship;
    [SerializeField]
    private ExplosionFactory _explosionFactory;

    public float GameTime { get; private set; }
    public event System.Action GameOvered;

    private Vector2 _direction = Vector2.up;
    private Vector2 _flyDirection = Vector2.up;
    private Vector2 _course = Vector2.up;
    private ValueKeeper _acceleration = new ValueKeeper();
    private ValueKeeper _ricochet = new ValueKeeper();
    private Rect _field;
    private bool _shipAlive = true;

    private void Start()
    {
        _field = _camera.GetFrustumRect();
        _explosionFactory.Init(_field);
        StartGame();
    }

    public void StartGame()
    {
        _ship.gameObject.SetActive(true);
        _ship.position = Vector3.zero;
        _direction = Vector2.up;
        _flyDirection = Vector2.up;
        _course = Vector2.up;
        GameTime = 0f;
        _shipAlive = true;
        _explosionFactory.Begin();
    }

    private void Update()
    {
        if (_shipAlive)
        {
            GameTime += Time.deltaTime;
            ValueKeepersUpdate();
            ShipMovement();
            RicochetChecking();
            CheckDeath();
        }

        Debug.DrawRay(_ship.position, _course * 50f, Color.red);
        Debug.DrawRay(_ship.position, _direction * 5f, Color.green);
        Debug.DrawRay(_ship.position, _flyDirection / _speed * 5f, Color.blue);
    }

    private void ValueKeepersUpdate()
    {
        _acceleration.MinTime = _accelerationTime;
        _acceleration.Update(Time.deltaTime);
        _ricochet.MinTime = _ricochetTime;
        _ricochet.Update(Time.deltaTime);
    }

    private void ShipMovement()
    {
        var mousePos = _camera.ScreenToFrustumPoint(Input.mousePosition);
        _course = (mousePos - _ship.position).normalized;

        if (Input.GetMouseButton(0))
            _acceleration.Start();
        else
            _acceleration.Stop();

        if (_acceleration)
            _direction = Vector3.RotateTowards(_direction, _course, _rotateSpeed * Time.deltaTime, 0f);

        var seedMod = _acceleration || _ricochet ? _accelerationMod : 1f;

        _flyDirection += _direction * _speed * seedMod * Time.deltaTime;
        if (_flyDirection.magnitude > _speed * seedMod)
            _flyDirection -= _flyDirection.normalized * _speed * _accelerationMod * Time.deltaTime;

        _ship.rotation = Quaternion.LookRotation(_direction, Vector3.forward);
        _ship.position += (Vector3)(_flyDirection * Time.deltaTime);
    }

    private void RicochetChecking()
    {
        if (_ship.position.x > _field.xMax || _ship.position.x < _field.xMin)
        {
            _direction.Scale(HorizontalMirror);
            _flyDirection.Scale(HorizontalMirror);
            _ricochet.Start();
        }
        if (_ship.position.y > _field.yMax || _ship.position.y < _field.yMin)
        {
            _direction.Scale(VerticalMirror);
            _flyDirection.Scale(VerticalMirror);
            _ricochet.Start();
        }
    }

    private void CheckDeath()
    {
        if (_explosionFactory.CheckIntersect(_ship.position))
        {
            _shipAlive = false;
            _ship.gameObject.SetActive(false);
            _explosionFactory.Stop();
            if (GameOvered != null)
                GameOvered();
        }
    }
}
