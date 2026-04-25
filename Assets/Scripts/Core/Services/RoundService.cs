public class RoundService
{
    private readonly GameSignals _signals;
    private readonly BallReserveService _ballReserve;

    public int CurrentRound { get; private set; } = 0;
    public int BallsPerRound { get; private set; } = 3;

    public RoundService(GameSignals signals, BallReserveService ballReserve)
    {
        _signals = signals;
        _ballReserve = ballReserve;
    }

    public void SetBallsPerRound(int amount)
    {
        BallsPerRound = amount;
    }

    public void StartNextRound()
    {
        CurrentRound++;

        _ballReserve.SetStartingReserve(BallsPerRound);

        _signals.RaiseRoundChanged(CurrentRound);
        _signals.RaiseRoundStarted();
    }

    public void EndRound()
    {
        _signals.RaiseRoundEnded();
    }
}