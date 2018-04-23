using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraHandler : MonoBehaviour
{
    private Camera _camera;
    public Camera Camera
    {
        get
        {
            if (_camera == null)
                _camera = GetComponent<Camera>();
            return _camera;
        }
    }

    Plane _plane = new Plane(Vector3.back, Vector3.zero);
    Vector3[] _frustumCorners;

    public Vector3 ScreenToFrustumPoint(Vector3 point)
    {
        var ray = this.Camera.ScreenPointToRay(point);
        float enter;
        _plane.Raycast(ray, out enter);
        return ray.GetPoint(enter);
    }

    private void Awake()
    {
        _frustumCorners = new Vector3[4];
        this.Camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), -this.Camera.transform.position.z, Camera.MonoOrStereoscopicEye.Mono, _frustumCorners);
    }

    private void Update()
    {
        for (int i = 0; i < _frustumCorners.Length; i++)
            Debug.DrawLine(_frustumCorners[i] + this.Camera.transform.position, _frustumCorners[(i + 1) % _frustumCorners.Length] + this.Camera.transform.position, Color.blue);
    }

    public Rect GetFrustumRect()
    {
        var x = float.MaxValue;
        var y = float.MaxValue;
        var width = 0f;
        var height = 0f;
        for (int i = 0; i < _frustumCorners.Length; i++)
        {
            var corn = _frustumCorners[i];
            var nextCorn = _frustumCorners[(i + 1) % _frustumCorners.Length];
            x = Mathf.Min(x, corn.x);
            y = Mathf.Min(y, corn.y);
            width = Mathf.Max(width, Mathf.Abs(nextCorn.x - corn.x));
            height = Mathf.Max(height, Mathf.Abs(nextCorn.y - corn.y));
        }
        return new Rect(x, y, width, height);
    }
}
