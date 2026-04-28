public class GameLoopController
{
    private readonly StateMachineService _stateMachine;
    private readonly BallReserveService _ballReserve;
    private readonly BallLifecycleService _ballLifecycle;
    private readonly RoundService _rounds;

    public GameLoopController(
        StateMachineService stateMachine,
        BallReserveService ballReserve,
        BallLifecycleService ballLifecycle,
        RoundService rounds)
    {
        _stateMachine = stateMachine;
        _ballReserve = ballReserve;
        _ballLifecycle = ballLifecycle;
        _rounds = rounds;
    }

    public void StartGame()
    {
        _stateMachine.EnterState(GameState.ShopBuild);
    }

    public void StartRound()
    {
        _rounds.StartNextRound();
        _stateMachine.EnterState(GameState.WaitingForBall);
    }

    public void OnBallLoaded()
    {
        _stateMachine.EnterState(GameState.BallLoaded);
    }

    public void OnLaunchStarted()
    {
        _stateMachine.EnterState(GameState.BallLaunching);
    }

    public void OnBallLaunched()
    {
        _ballLifecycle.LaunchLoadedBall();
        _stateMachine.EnterState(GameState.BallInPlay);
    }

    public void OnBallDrained(BallRuntimeData drainedBall)
    {
        _stateMachine.EnterState(GameState.ResolvingDrain);
        _ballLifecycle.DrainBall(drainedBall);

        if (_ballReserve.HasReserve())
        {
            _stateMachine.EnterState(GameState.WaitingForBall);
            return;
        }

        if (_ballLifecycle.GetActiveBallCount() <= 0)
        {
            EndRoundAndEnterShop();
            return;
        }

        _stateMachine.EnterState(GameState.BallInPlay);
    }

    public bool CanPrepareNextBall()
    {
        return _stateMachine.IsInState(GameState.WaitingForBall)
            && _ballReserve.HasReserve()
            && _ballLifecycle.LoadedBall == null;
    }

    public bool TryConsumeReserveForNextBall()
    {
        if (!CanPrepareNextBall())
            return false;

        return _ballReserve.TryConsumeOne();
    }

    private void EndRoundAndEnterShop()
    {
        _rounds.EndRound();
        _stateMachine.EnterState(GameState.ShopBuild);
    }

    public void EnterBoardEdit()
    {
        _stateMachine.EnterState(GameState.BoardEdit);
    }

    public void EnterShopBuild()
    {
        _stateMachine.EnterState(GameState.ShopBuild);
    }
}