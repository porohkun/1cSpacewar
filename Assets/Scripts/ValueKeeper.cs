using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ValueKeeper
{
    public float MinTime { get; set; }
    public bool Value { get { return _remainingTime > 0f; } }

    private float _remainingTime;
    private float _elapsedTime;

    public void Start()
    {
        if (!Value)
            _elapsedTime = 0f;
        _remainingTime = MinTime;
    }

    public void Stop()
    {
        if (_elapsedTime >= MinTime)
            _remainingTime = -1f;
    }

    public void Update(float deltaTime)
    {
        _remainingTime -= deltaTime;
        _elapsedTime += deltaTime;
    }

    public static implicit operator bool(ValueKeeper keeper)
    {
        return keeper.Value;
    }
}
