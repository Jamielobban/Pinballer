public class RoundService
{
    private readonly GameSignals _signals;

    public int CurrentRound { get; private set; }

    public RoundService(GameSignals signals)
    {
        _signals = signals;
    }

    public void StartNextRound()
    {
        CurrentRound++;

        _signals.RaiseRoundChanged(CurrentRound);
        _signals.RaiseRoundStarted();
    }

    public void EndRound()
    {
        _signals.RaiseRoundEnded();
    }

    public int GetCurrentTargetScore()
    {
        if (CurrentRound == 1)
            return 1;

        return 100 + ((CurrentRound - 1) * 75);
    }
}