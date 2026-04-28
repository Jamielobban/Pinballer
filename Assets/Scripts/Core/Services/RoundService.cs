public class RoundService
{
    private readonly GameSignals _signals;
    private readonly BallReserveService _ballReserve;
    private readonly StatService _stats;
    private readonly BallInventoryService _ballInventory;

    public int CurrentRound { get; private set; } = 0;

    public RoundService(
        GameSignals signals,
        BallReserveService ballReserve,
        StatService stats,
        BallInventoryService ballInventory)
    {
        _signals = signals;
        _ballReserve = ballReserve;
        _stats = stats;
        _ballInventory = ballInventory;
    }

    public void StartNextRound()
    {
        CurrentRound++;

        _ballInventory.ResetQueue();

        int ballsThisRound = _stats.GetBallsPerRound();
        _ballReserve.SetStartingReserve(ballsThisRound);

        _signals.RaiseRoundChanged(CurrentRound);
        _signals.RaiseRoundStarted();
    }

    public void EndRound()
    {
        _signals.RaiseRoundEnded();
    }
}