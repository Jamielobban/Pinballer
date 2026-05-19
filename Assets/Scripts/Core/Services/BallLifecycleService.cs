using System.Collections.Generic;

public class BallLifecycleService
{
    private readonly GameSession _session;
    private readonly GameSignals _signals;

    private readonly List<BallRuntimeData> _activeBalls = new();

    public BallRuntimeData LoadedBall { get; private set; }

    public BallRuntimeData CurrentFollowBall
    {
        get
        {
            if (LoadedBall != null)
                return LoadedBall;

            if (_activeBalls.Count > 0)
                return _activeBalls[0];

            return null;
        }
    }

    public BallLifecycleService(GameSession session, GameSignals signals)
    {
        _session = session;
        _signals = signals;
    }

    public void RegisterSpawn(BallRuntimeData ball)
    {
        if (ball == null)
            return;

        if (!_activeBalls.Contains(ball))
            _activeBalls.Add(ball);
    }

    public void SetLoadedBall(BallRuntimeData ball)
    {
        LoadedBall = ball;
    }

    public void LaunchLoadedBall()
    {
        if (LoadedBall == null)
            return;

        LoadedBall.IsLoaded = false;
        LoadedBall.IsInPlay = true;

        RegisterSpawn(LoadedBall);

        LoadedBall = null;
    }

    public void DrainBall(BallRuntimeData ball)
    {
        if (ball == null)
            return;

        ball.IsLoaded = false;
        ball.IsInPlay = false;

        if (LoadedBall == ball)
            LoadedBall = null;

        _activeBalls.Remove(ball);
    }

    public int GetActiveBallCount()
    {
        return _activeBalls.Count;
    }

    public IReadOnlyList<BallRuntimeData> GetActiveBalls()
    {
        return _activeBalls;
    }

    public void Clear()
    {
        LoadedBall = null;
        _activeBalls.Clear();
    }
}