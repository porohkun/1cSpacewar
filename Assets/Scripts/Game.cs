using System;
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
    private float _rotateSpeed = 1f;
    [SerializeField]
    private float _ricochetTime = 0.3f;
    [SerializeField]
    private CameraHandler _camera;
    [SerializeField]
    private Transform _ship;

    private Vector2 _direction = Vector2.up;
    private Vector2 _course = Vector2.up;
    private bool _acceleration = false;
    private bool _ricochet { get { return _ricochetElapsedTime > 0f; } }
    private Rect _field;
    float _ricochetElapsedTime;

    private void Start()
    {
        _field = _camera.GetFrustumRect();
    }

    private void Update()
    {
        ShipMovement();
        RicochetChecking();

        Debug.DrawRay(_ship.position, _course * 50f, Color.red);
        Debug.DrawRay(_ship.position, _direction * 5f, Color.green);
    }

    private void ShipMovement()
    {
        var mousePos = _camera.ScreenToFrustumPoint(Input.mousePosition);
        _course = (mousePos - _ship.position).normalized;

        _acceleration = Input.GetMouseButton(0);
        if (_acceleration)
            _direction = Vector3.RotateTowards(_direction, _course, _rotateSpeed * Time.deltaTime, 0f);

        _ship.rotation = Quaternion.LookRotation(_direction, Vector3.forward);
        _ship.position += (Vector3)(_direction * _speed * Time.deltaTime * (_acceleration || _ricochet ? _accelerationMod : 1f));
    }

    private void RicochetChecking()
    {
        if (_ship.position.x > _field.xMax || _ship.position.x < _field.xMin)
        {
            _direction.Scale(HorizontalMirror);
            _ricochetElapsedTime = _ricochetTime;
        }
        if (_ship.position.y > _field.yMax || _ship.position.y < _field.yMin)
        {
            _direction.Scale(VerticalMirror);
            _ricochetElapsedTime = _ricochetTime;
        }
        _ricochetElapsedTime -= Time.deltaTime;
    }
}
