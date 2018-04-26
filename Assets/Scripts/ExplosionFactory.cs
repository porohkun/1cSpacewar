using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ExplosionFactory : MonoBehaviour
{
    [SerializeField]
    private bool _generateExplosions = true;
    [SerializeField]
    private Explosion _explosionPrefab;
    [SerializeField]
    private float _startTimeBetweenExplosions = 5f;
    [SerializeField]
    private float _explosionRecalcTime = 5f;
    [SerializeField]
    private float _explosionAccelerationTimeMod = 0.9f;

    private float _timeBetweenExplosions = 5f;
    private List<Explosion> _explosions = new List<Explosion>();
    private Rect _field;

    public void Init(Rect field)
    {
        _field = field;
    }

    public void Begin()
    {
        _timeBetweenExplosions = _startTimeBetweenExplosions;
        _explosions.ForEach(e => e.gameObject.SetActive(false));

        StartCoroutine(ExplosionAccelerationRoutine());
        StartCoroutine(ExplosionCreationRoutine());
    }

    public void Stop()
    {
        StopAllCoroutines();
        _explosions.ForEach(e => e.enabled = false);
    }
    public bool CheckIntersect(Vector3 position)
    {
        return _explosions.Any(e => !e.IsSafe && e.Contains(position));
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
                var explosion = GetExplosion();
                explosion.Begin(new Vector2(Random.Range(_field.xMin, _field.xMax), Random.Range(_field.yMin, _field.yMax)));
            }
            yield return new WaitForSeconds(_timeBetweenExplosions);
        }
    }

    private Explosion GetExplosion()
    {
        var explosion = _explosions.FirstOrDefault(e => !e.gameObject.activeSelf);
        if (explosion == null)
        {
            explosion = Instantiate(_explosionPrefab, transform);
            _explosions.Add(explosion);
        }
        return explosion;
    }

}
