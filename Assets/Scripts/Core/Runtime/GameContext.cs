public class GameContext
{
    public GameSignals Signals { get; private set; }
    public GameSession Session { get; private set; }
    public StateMachineService StateMachine { get; private set; }
    public EconomyService Economy { get; private set; }
    public UpgradeService Upgrades { get; private set; }
    public ModifierService Modifiers { get; private set; }
    public StatService Stats { get; private set; }
    public BallReserveService BallReserve { get; private set; }
    public BallLifecycleService BallLifecycle { get; private set; }
    public RoundService Rounds { get; private set; }
    public InventoryService Inventory { get; private set; }
    public GameLoopController Loop { get; private set; }
    public BallInventoryService BallInventory { get; private set; }

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
        BallInventoryService ballInventory)
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
    }
}