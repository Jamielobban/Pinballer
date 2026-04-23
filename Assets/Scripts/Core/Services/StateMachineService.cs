public class StateMachineService
{
    private readonly GameSession _session;
    private readonly GameSignals _signals;

    public StateMachineService(GameSession session, GameSignals signals)
    {
        _session = session;
        _signals = signals;
    }

    public GameState CurrentState => _session.CurrentState;

    public void EnterState(GameState newState)
    {
        _session.SetState(newState);
        _signals.RaiseGameStateChanged(newState);
    }

    public bool IsInState(GameState state)
    {
        return _session.CurrentState == state;
    }
}