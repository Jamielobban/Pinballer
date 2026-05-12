public class RoundService
{
    private readonly GameSignals _signals;
    private readonly BallReserveService _ballReserve;
    private readonly StatService _stats;
    private readonly BallInventoryService _ballInventory;

    public int CurrentRound { get; private set; }

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
        UnityEngine.Debug.Log("Testing");
        UnityEngine.Debug.Log(CurrentRound);
        CurrentRound++;

        UnityEngine.Debug.Log(CurrentRound);
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

    public int GetCurrentTargetScore()
    {
        if (CurrentRound == 1) return 1;
        else return 100 + ((CurrentRound - 1) * 75);
    }
}