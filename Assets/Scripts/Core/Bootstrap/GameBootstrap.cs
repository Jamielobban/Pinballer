using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public static GameContext Context { get; private set; }

    [Header("RNG")]
    [SerializeField] private bool useFixedSeed = true;
    [SerializeField] private int fixedSeed = 12345;

    [Header("Starting Values")]
    [SerializeField] private int startingMoney = 0;

    [Header("Balls")]
    [SerializeField] private List<BallDefinition> startingBalls = new List<BallDefinition>();

    [Header("Data")]
    [SerializeField] private List<UpgradeDefinition> upgradeDefinitions = new List<UpgradeDefinition>();
    [SerializeField] private List<ModifierDefinition> startingModifiers = new List<ModifierDefinition>();

    private void Awake()
    {
        BuildContext();
        ApplyStartingData();
        StartGameLoop();
    }

    private void BuildContext()
    {
        GameSignals signals = new GameSignals();
        GameSession session = new GameSession();

        int seed = useFixedSeed
            ? fixedSeed
            : Random.Range(int.MinValue, int.MaxValue);

        RNGService rng = new RNGService(seed);

        Debug.Log("Run Seed: " + seed);

        StateMachineService stateMachine = new StateMachineService(session, signals);
        EconomyService economy = new EconomyService(session, signals);
        UpgradeService upgrades = new UpgradeService(economy, signals);
        ModifierService modifiers = new ModifierService(signals);
        StatService stats = new StatService(upgrades, modifiers, upgradeDefinitions);

        BallReserveService ballReserve = new BallReserveService();
        BallLifecycleService ballLifecycle = new BallLifecycleService(session, signals);
        InventoryService inventory = new InventoryService(signals);
        BallInventoryService ballInventory = new BallInventoryService(signals);
        BallLotteryService ballLottery = new BallLotteryService(ballInventory, rng);

        RoundService rounds = new RoundService(signals);
        ScoreService score = new ScoreService(session, signals);

        GameLoopController loop = new GameLoopController(
            stateMachine,
            ballReserve,
            ballLifecycle,
            rounds,
            ballLottery,
            stats
        );

        Context = new GameContext(
            signals,
            session,
            stateMachine,
            economy,
            upgrades,
            modifiers,
            stats,
            ballReserve,
            ballLifecycle,
            rounds,
            inventory,
            loop,
            ballInventory,
            score,
            rng,
            ballLottery
        );
    }

    private void ApplyStartingData()
    {
        Context.Economy.SetStartingMoney(startingMoney);

        for (int i = 0; i < startingModifiers.Count; i++)
        {
            if (startingModifiers[i] != null)
                Context.Modifiers.AddModifier(startingModifiers[i]);
        }

        for (int i = 0; i < startingBalls.Count; i++)
        {
            if (startingBalls[i] != null)
                Context.BallInventory.AddBall(startingBalls[i]);
        }
    }

    private void StartGameLoop()
    {
        Context.Loop.StartGame();
    }
}