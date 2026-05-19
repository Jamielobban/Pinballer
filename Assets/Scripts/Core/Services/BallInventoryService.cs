using System;
using System.Collections.Generic;

public class BallInventoryService
{
    private readonly GameSignals _signals;
    private readonly List<BallRuntimeData> _ownedBalls = new();

    public IReadOnlyList<BallRuntimeData> OwnedBalls => _ownedBalls;

    public event Action OnChanged;

    public BallInventoryService(GameSignals signals)
    {
        _signals = signals;
    }

    public void AddBall(BallDefinition definition)
    {
        if (definition == null)
            return;

        BallRuntimeData ball = new BallRuntimeData
        {
            BallId = _ownedBalls.Count + 1,
            Definition = definition,
            IsLoaded = false,
            IsInPlay = false,
            SizeMultiplier = definition.SizeMultiplier,
            SpeedMultiplier = 1f,
            ValueMultiplier = definition.ValueMultiplier
        };

        _ownedBalls.Add(ball);

        OnChanged?.Invoke();
        _signals.RaiseBallQueueChanged();
    }

    public void AddBall(BallRuntimeData ball)
    {
        if (ball == null)
            return;

        _ownedBalls.Add(ball);

        OnChanged?.Invoke();
        _signals.RaiseBallQueueChanged();
    }

    public void SwapBalls(int a, int b)
    {
        if (a < 0 || b < 0)
            return;

        if (a >= _ownedBalls.Count || b >= _ownedBalls.Count)
            return;

        (_ownedBalls[a], _ownedBalls[b]) = (_ownedBalls[b], _ownedBalls[a]);

        OnChanged?.Invoke();
        _signals.RaiseBallQueueChanged();
    }
}