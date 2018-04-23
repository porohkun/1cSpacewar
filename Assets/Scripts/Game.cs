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
    private float _accelerationTime = 0.5f;
    [SerializeField]
    private float _ricochetTime = 0.3f;
    [SerializeField]
    private float _startTimeBetweenExplosions = 5f;
    [SerializeField]
    private float _explosionRecalcTime = 5f;
    [SerializeField]
    private float _explosionAccelerationTimeMod = 0.9f;
    [SerializeField]
    private CameraHandler _camera;
    [SerializeField]
    private Transform _ship;
    [SerializeField]
    private Explosion _explosionPrefab;

    private Vector2 _direction = Vector2.up;
    private Vector2 _course = Vector2.up;
    private ValueKeeper _acceleration = new ValueKeeper();
    private ValueKeeper _ricochet = new ValueKeeper();
    private Rect _field;
    private float _gameTime;
    private float _timeBetweenExplosions = 5f;
    private List<Explosion> _explosions = new List<Explosion>();

    private void Start()
    {
        _field = _camera.GetFrustumRect();
        StartGame();
    }

    public void StartGame()
    {
        _timeBetweenExplosions = _startTimeBetweenExplosions;
        StartCoroutine(ExplosionAccelerationRoutine());
        StartCoroutine(ExplosionCreationRoutine());
    }

    private IEnumerator ExplosionAccelerationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_explosionRecalcTime);
            _timeBetweenExplosions *= _explosionAccelerationTimeMod;
        }
    }

    private IEnumerator ExplosionCreationRoutine()
    {
        while (true)
        {
            Explosion expl = GetExplosion();
            expl.gameObject.SetActive(true);
            expl.Begin();
            expl.transform.position = new Vector2(Random.Range(_field.xMin, _field.xMax), Random.Range(_field.yMin, _field.yMax));
            yield return new WaitForSeconds(_timeBetweenExplosions);
        }
    }

    private Explosion GetExplosion()
    {
        Explosion explosion = _explosions.FirstOrDefault(e => !e.gameObject.activeSelf);
        if (explosion == null)
        {
            explosion = Instantiate(_explosionPrefab, transform);
            _explosions.Add(explosion);
        }
        return explosion;
    }

    private void Update()
    {
        _gameTime += Time.deltaTime;
        ValueKeepersUpdate();
        ShipMovement();
        RicochetChecking();

        Debug.DrawRay(_ship.position, _course * 50f, Color.red);
        Debug.DrawRay(_ship.position, _direction * 5f, Color.green);
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

        _ship.rotation = Quaternion.LookRotation(_direction, Vector3.forward);
        _ship.position += (Vector3)(_direction * _speed * Time.deltaTime * (_acceleration || _ricochet ? _accelerationMod : 1f));
    }

    private void RicochetChecking()
    {
        if (_ship.position.x > _field.xMax || _ship.position.x < _field.xMin)
        {
            _direction.Scale(HorizontalMirror);
            _ricochet.Start();
        }
        if (_ship.position.y > _field.yMax || _ship.position.y < _field.yMin)
        {
            _direction.Scale(VerticalMirror);
            _ricochet.Start();
        }
    }
}
