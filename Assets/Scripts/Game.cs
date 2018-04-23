using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class Game : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1f;
    [SerializeField]
    private float _accelerationMod = 2f;
    [SerializeField]
    private float _rotateSpeed = 1f;
    [SerializeField]
    private CameraHandler _camera;
    [SerializeField]
    private Transform _ship;

    private Vector2 _direction = Vector2.up;
    private Vector2 _course = Vector2.up;
    private bool _acceleration = false;

    private void Update()
    {
        var mousePos = _camera.ScreenToFrustumPoint(Input.mousePosition);
        _course = (mousePos - _ship.position).normalized;
        _direction = Vector3.RotateTowards(_direction, _course, _rotateSpeed * Time.deltaTime, 0f);

        Debug.DrawLine(_ship.position, mousePos, Color.red);
        Debug.DrawRay(_ship.position, _direction * 5f, Color.green);

        _ship.rotation = Quaternion.LookRotation(_direction, Vector3.forward);
        _ship.position += (Vector3)(_direction * _speed * Time.deltaTime * (_acceleration ? 1f : _accelerationMod));


        //transform.position = mousePos;
    }
}
