public class BallReserveService
{
    private readonly GameSession _session;
    private readonly GameSignals _signals;

    public BallReserveService(GameSession session, GameSignals signals)
    {
        _session = session;
        _signals = signals;
    }

    public int CurrentReserve => _session.BallsInReserve;

    public void SetStartingReserve(int amount)
    {
        _session.SetReserve(amount);
        _signals.RaiseBallReserveChanged(_session.BallsInReserve);
    }

    public bool HasReserve()
    {
        return _session.BallsInReserve > 0;
    }

    public bool TryConsumeOne()
    {
        if (_session.BallsInReserve <= 0)
            return false;

        _session.ConsumeReserve(1);
        _signals.RaiseBallReserveChanged(_session.BallsInReserve);
        return true;
    }

    public void AddBalls(int amount)
    {
        if (amount <= 0)
            return;

        _session.AddReserve(amount);
        _signals.RaiseBallReserveChanged(_session.BallsInReserve);
    }
}