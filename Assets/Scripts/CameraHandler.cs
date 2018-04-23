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
    
    public Vector3 ScreenToFrustumPoint(Vector3 point)
    {
        var ray = this.Camera.ScreenPointToRay(point);
        float enter;
        _plane.Raycast(ray, out enter);
        return ray.GetPoint(enter);
    }

    void Update()
    {
        Vector3[] frustumCorners = new Vector3[4];
        this.Camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), -this.Camera.transform.position.z, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        for (int i = 0; i < frustumCorners.Length; i++)
            Debug.DrawLine(frustumCorners[i] + this.Camera.transform.position, frustumCorners[(i + 1) % frustumCorners.Length] + this.Camera.transform.position, Color.blue);
    }
}
