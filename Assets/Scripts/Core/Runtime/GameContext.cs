public class GameContext
{
    public GameSignals Signals { get; }
    public GameSession Session { get; }
    public StateMachineService StateMachine { get; }
    public EconomyService Economy { get; }
    public UpgradeService Upgrades { get; }
    public ModifierService Modifiers { get; }
    public StatService Stats { get; }
    public BallReserveService BallReserve { get; }
    public BallLifecycleService BallLifecycle { get; }
    public RoundService Rounds { get; }
    public InventoryService Inventory { get; }
    public GameLoopController Loop { get; }
    public BallInventoryService BallInventory { get; }
    public ScoreService Score { get; }
    public RNGService RNG { get; }
    public BallLotteryService BallLottery { get; }

    public GameContext(
        GameSignals signals,
        GameSession session,
        StateMachineService stateMachine,
        EconomyService economy,
        UpgradeService upgrades,
        ModifierService modifiers,
        StatService stats,
        BallReserveService ballReserve,
        BallLifecycleService ballLifecycle,
        RoundService rounds,
        InventoryService inventory,
        GameLoopController loop,
        BallInventoryService ballInventory,
        ScoreService score,
        RNGService rng,
        BallLotteryService ballLottery)
    {
        Signals = signals;
        Session = session;
        StateMachine = stateMachine;
        Economy = economy;
        Upgrades = upgrades;
        Modifiers = modifiers;
        Stats = stats;
        BallReserve = ballReserve;
        BallLifecycle = ballLifecycle;
        Rounds = rounds;
        Inventory = inventory;
        Loop = loop;
        BallInventory = ballInventory;
        Score = score;
        RNG = rng;
        BallLottery = ballLottery;
    }
}