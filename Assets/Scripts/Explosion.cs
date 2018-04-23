using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _renderer;
    [SerializeField]
    private Material _safeExplosionMaterial;
    [SerializeField]
    private Material _dangerExplosionMaterial;
    [SerializeField]
    private float _safeTime = 1f;
    [SerializeField]
    private float _fullTime = 3f;
    [SerializeField]
    private float _explodeSpeed = 15f;

    public bool IsSafe { get { return _elapsedTime < _safeTime; } }

    private float _diameter;
    private float _elapsedTime;

    public void Begin()
    {
        _elapsedTime = 0f;
        _diameter = 0f;
        UpdateVisual();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        _diameter += _explodeSpeed * Time.deltaTime;
        UpdateVisual();

        if (_elapsedTime >= _fullTime)
            gameObject.SetActive(false);
    }

    private void UpdateVisual()
    {
        _renderer.material = _elapsedTime < _safeTime ? _safeExplosionMaterial : _dangerExplosionMaterial;
        transform.localScale = Vector3.one * _diameter;
    }
}
