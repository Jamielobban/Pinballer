using System;
using System.Collections.Generic;
using System.Linq;

public class BallReserveService
{
    private readonly Queue<BallRuntimeData> _reserve = new();

    public event Action OnReserveChanged;

    public bool HasReserve()
    {
        return _reserve.Count > 0;
    }

    public void SetReserve(IEnumerable<BallRuntimeData> balls)
    {
        _reserve.Clear();

        foreach (var ball in balls)
        {
            _reserve.Enqueue(ball);
        }

        OnReserveChanged?.Invoke();

        if (GameBootstrap.Context != null)
            GameBootstrap.Context.Signals.RaiseBallQueueChanged();
    }

    public bool TryConsumeOne(out BallRuntimeData ball)
    {
        ball = null;

        if (_reserve.Count <= 0)
            return false;

        ball = _reserve.Dequeue();

        OnReserveChanged?.Invoke();

        if (GameBootstrap.Context != null)
            GameBootstrap.Context.Signals.RaiseBallQueueChanged();

        return true;
    }

    public IReadOnlyList<BallRuntimeData> GetReserveSnapshot()
    {
        return _reserve.ToList();
    }

    public void Clear()
    {
        _reserve.Clear();

        OnReserveChanged?.Invoke();

        if (GameBootstrap.Context != null)
            GameBootstrap.Context.Signals.RaiseBallQueueChanged();
    }

    public void AddToReserve(BallRuntimeData ball)
    {
        if (ball == null)
            return;

        _reserve.Enqueue(ball);

        OnReserveChanged?.Invoke();

        if (GameBootstrap.Context != null)
            GameBootstrap.Context.Signals.RaiseBallQueueChanged();
    }
}