public class GameLoopController
{
    private readonly StateMachineService _stateMachine;
    private readonly BallReserveService _ballReserve;
    private readonly BallLifecycleService _ballLifecycle;
    private readonly RoundService _rounds;
    private readonly BallLotteryService _ballLottery;
    private readonly StatService _stats;

    public GameLoopController(
        StateMachineService stateMachine,
        BallReserveService ballReserve,
        BallLifecycleService ballLifecycle,
        RoundService rounds,
        BallLotteryService ballLottery,
        StatService stats)
    {
        _stateMachine = stateMachine;
        _ballReserve = ballReserve;
        _ballLifecycle = ballLifecycle;
        _rounds = rounds;
        _ballLottery = ballLottery;
        _stats = stats;
    }

    public void StartGame()
    {
        _stateMachine.EnterState(GameState.ShopBuild);
    }

   public void StartRound()
    {
        _rounds.StartNextRound();

        int targetScore = _rounds.GetCurrentTargetScore();
        GameBootstrap.Context.Score.StartRound(targetScore);

        _ballReserve.Clear();

        _stateMachine.EnterState(GameState.LotteryDraw);
    }

    public void FinishLotteryDraw()
    {
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

        bool hasReserve = _ballReserve.HasReserve();
        bool hasActiveBalls = _ballLifecycle.GetActiveBallCount() > 0;
        bool hasLoadedBall = _ballLifecycle.LoadedBall != null;

        if (hasActiveBalls || hasLoadedBall)
        {
            _stateMachine.EnterState(GameState.BallInPlay);
            return;
        }

        if (hasReserve)
        {
            _stateMachine.EnterState(GameState.WaitingForBall);
            return;
        }

        EndRoundAndEnterShop();
    }

    public bool CanPrepareNextBall()
    {
        return _stateMachine.IsInState(GameState.WaitingForBall)
            && _ballReserve.HasReserve()
            && _ballLifecycle.LoadedBall == null;
    }

    public bool TryConsumeReserveForNextBall(out BallRuntimeData ball)
    {
        ball = null;

        if (!CanPrepareNextBall())
            return false;

        return _ballReserve.TryConsumeOne(out ball);
    }

    private void EndRoundAndEnterShop()
    {
        _rounds.EndRound();

        if (!GameBootstrap.Context.Score.HasMetTarget())
        {
            _stateMachine.EnterState(GameState.GameOver);
            return;
        }

        // Do not auto-enter shop.
        // RoundResultPanelView will send player to ShopBuild.
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