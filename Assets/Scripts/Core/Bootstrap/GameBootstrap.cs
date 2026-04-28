using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public static GameContext Context { get; private set; }

    [Header("Starting Values")]
    [SerializeField] private int startingMoney = 0;
    [SerializeField] private int startingBallReserve = 3;

    [Header("Balls")]
    [SerializeField] private BallDefinition defaultBallDefinition;
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

        StateMachineService stateMachine = new StateMachineService(session, signals);
        EconomyService economy = new EconomyService(session, signals);
        UpgradeService upgrades = new UpgradeService(economy, signals);
        ModifierService modifiers = new ModifierService(signals);
        StatService stats = new StatService(upgrades, modifiers, upgradeDefinitions);
        BallReserveService ballReserve = new BallReserveService(session, signals);
        BallLifecycleService ballLifecycle = new BallLifecycleService(session, signals);
        InventoryService inventory = new InventoryService(signals);
        BallInventoryService ballInventory = new BallInventoryService(signals);
        RoundService rounds = new RoundService(signals, ballReserve, stats, ballInventory);

        GameLoopController loop = new GameLoopController(
            stateMachine,
            ballReserve,
            ballLifecycle,
            rounds
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
            ballInventory
        );
    }

    private void ApplyStartingData()
    {
        Context.Economy.SetStartingMoney(startingMoney);
        Context.BallReserve.SetStartingReserve(startingBallReserve);

        for (int i = 0; i < startingModifiers.Count; i++)
        {
            if (startingModifiers[i] != null)
            {
                Context.Modifiers.AddModifier(startingModifiers[i]);
            }
        }

        for (int i = 0; i < startingBalls.Count; i++)
        {
            Context.BallInventory.AddBall(startingBalls[i]);
        }
    }

    private void StartGameLoop()
    {
        Context.Loop.StartGame();
    }
}