using System;

public sealed class GameSignals
{
    public event Action<GameState> GameStateChanged;
    public event Action<int> MoneyChanged;
    public event Action<int> BallReserveChanged;
    public event Action<BallRuntimeData> BallSpawned;
    public event Action<BallRuntimeData> BallLoaded;
    public event Action<BallRuntimeData> BallLaunched;
    public event Action<BallRuntimeData> BallDrained;
    public event Action<HitEventData> HitScored;
    public event Action<UpgradeDefinition, int> UpgradePurchased;
    public event Action<ModifierDefinition> ModifierApplied;

    public void RaiseGameStateChanged(GameState state) => GameStateChanged?.Invoke(state);
    public void RaiseMoneyChanged(int amount) => MoneyChanged?.Invoke(amount);
    public void RaiseBallReserveChanged(int reserve) => BallReserveChanged?.Invoke(reserve);
    public void RaiseBallSpawned(BallRuntimeData ball) => BallSpawned?.Invoke(ball);
    public void RaiseBallLoaded(BallRuntimeData ball) => BallLoaded?.Invoke(ball);
    public void RaiseBallLaunched(BallRuntimeData ball) => BallLaunched?.Invoke(ball);
    public void RaiseBallDrained(BallRuntimeData ball) => BallDrained?.Invoke(ball);
    public void RaiseHitScored(HitEventData hit) => HitScored?.Invoke(hit);
    public void RaiseUpgradePurchased(UpgradeDefinition definition, int level) => UpgradePurchased?.Invoke(definition, level);
    public void RaiseModifierApplied(ModifierDefinition definition) => ModifierApplied?.Invoke(definition);
}