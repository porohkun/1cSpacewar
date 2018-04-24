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
    [SerializeField]
    private bool _generateExplosions = true;

    public float GameTime { get { return _gameTime; } }
    public event System.Action GameOvered;

    private Vector2 _direction = Vector2.up;
    private Vector2 _flyDirection = Vector2.up;
    private Vector2 _course = Vector2.up;
    private ValueKeeper _acceleration = new ValueKeeper();
    private ValueKeeper _ricochet = new ValueKeeper();
    private Rect _field;
    private float _gameTime;
    private float _timeBetweenExplosions = 5f;
    private List<Explosion> _explosions = new List<Explosion>();
    private bool _shipAlive = true;

    private void Start()
    {
        _field = _camera.GetFrustumRect();
        StartGame();
    }

    public void StartGame()
    {
        _ship.gameObject.SetActive(true);
        _ship.position = Vector3.zero;
        _direction = Vector2.up;
        _course = Vector2.up;
        _gameTime = 0f;
        _timeBetweenExplosions = _startTimeBetweenExplosions;
        _explosions.ForEach(e => e.gameObject.SetActive(false));
        _shipAlive = true;
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
            if (_generateExplosions)
            {
                Explosion expl = GetExplosion();
                expl.Begin(new Vector2(Random.Range(_field.xMin, _field.xMax), Random.Range(_field.yMin, _field.yMax)));
            }
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
        if (_shipAlive)
        {
            _gameTime += Time.deltaTime;
            ValueKeepersUpdate();
            ShipMovement();
            RicochetChecking();
            CheckDeath();
        }

        Debug.DrawRay(_ship.position, _course * 50f, Color.red);
        Debug.DrawRay(_ship.position, _direction * 5f, Color.green);
        Debug.DrawRay(_ship.position, _flyDirection * 5f, Color.blue);
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

        _flyDirection = (_flyDirection + _direction * _accelerationPow * Time.deltaTime).normalized;

        _ship.rotation = Quaternion.LookRotation(_direction, Vector3.forward);
        _ship.position += (Vector3)(_flyDirection * _speed * Time.deltaTime * (_acceleration || _ricochet ? _accelerationMod : 1f));
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
        foreach (var explosion in _explosions.Where(e => !e.IsSafe))
            if (explosion.Contains(_ship.position))
            {
                _shipAlive = false;
                _ship.gameObject.SetActive(false);
                StopAllCoroutines();
                _explosions.ForEach(e => e.enabled = false);
                if (GameOvered != null)
                    GameOvered();
                break;
            }
    }
}
