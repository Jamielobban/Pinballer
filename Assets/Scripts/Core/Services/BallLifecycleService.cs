using System.Collections.Generic;

public class BallLifecycleService
{
    private readonly GameSession _session;
    private readonly GameSignals _signals;
    private readonly List<BallRuntimeData> _activeBalls = new List<BallRuntimeData>();

    public BallRuntimeData LoadedBall { get; private set; }
    public IReadOnlyList<BallRuntimeData> ActiveBalls => _activeBalls;

    public BallRuntimeData CurrentFollowBall { get; private set; }
    public BallLifecycleService(GameSession session, GameSignals signals)
    {
        _session = session;
        _signals = signals;
    }

    public void RegisterSpawn(BallRuntimeData ball)
    {
        if (ball == null)
            return;

        _signals.RaiseBallSpawned(ball);
    }

    public void SetLoadedBall(BallRuntimeData ball)
    {
        if (ball == null)
            return;

        ball.IsLoaded = true;
        ball.IsInPlay = false;
        LoadedBall = ball;

        _signals.RaiseBallLoaded(ball);
    }

    public void LaunchLoadedBall()
    {
        if (LoadedBall == null)
            return;

        LoadedBall.IsLoaded = false;
        LoadedBall.IsInPlay = true;

        _activeBalls.Add(LoadedBall);
        CurrentFollowBall = LoadedBall;
        _session.IncrementActiveBalls();

        _signals.RaiseBallLaunched(LoadedBall);
        LoadedBall = null;
    }

    public void DrainBall(BallRuntimeData ball)
    {
        if (ball == null)
            return;

        if (_activeBalls.Remove(ball))
        {
            _session.DecrementActiveBalls();
        }

        ball.IsLoaded = false;
        ball.IsInPlay = false;

        _signals.RaiseBallDrained(ball);
        if (CurrentFollowBall == ball)
        {
            CurrentFollowBall = _activeBalls.Count > 0 ? _activeBalls[_activeBalls.Count - 1] : null;
        }
    }

    public int GetActiveBallCount()
    {
        return _activeBalls.Count;
    }
}