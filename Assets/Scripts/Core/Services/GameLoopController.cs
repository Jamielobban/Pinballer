public class GameLoopController
{
    private readonly StateMachineService _stateMachine;
    private readonly BallReserveService _ballReserve;
    private readonly BallLifecycleService _ballLifecycle;

    public GameLoopController(
        StateMachineService stateMachine,
        BallReserveService ballReserve,
        BallLifecycleService ballLifecycle)
    {
        _stateMachine = stateMachine;
        _ballReserve = ballReserve;
        _ballLifecycle = ballLifecycle;
    }

    public void StartGame()
    {
        _stateMachine.EnterState(GameState.Boot);
        TryPrepareNextBall();
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
            TryPrepareNextBall();
            return;
        }

        if (_ballLifecycle.GetActiveBallCount() <= 0)
        {
            _stateMachine.EnterState(GameState.GameOver);
        }
        else
        {
            _stateMachine.EnterState(GameState.BallInPlay);
        }
    }

    public bool CanPrepareNextBall()
    {
        return _ballReserve.HasReserve() && _ballLifecycle.LoadedBall == null;
    }

    public bool TryConsumeReserveForNextBall()
    {
        if (!CanPrepareNextBall())
            return false;

        return _ballReserve.TryConsumeOne();
    }

    private void TryPrepareNextBall()
    {
        if (_ballLifecycle.LoadedBall != null)
        {
            _stateMachine.EnterState(GameState.BallLoaded);
            return;
        }

        if (_ballReserve.TryConsumeOne())
        {
            _stateMachine.EnterState(GameState.WaitingForBall);
        }
        else if (_ballLifecycle.GetActiveBallCount() <= 0)
        {
            _stateMachine.EnterState(GameState.GameOver);
        }
    }
}