using System.Collections.Generic;

public class UpgradeService
{
    private readonly EconomyService _economy;
    private readonly GameSignals _signals;
    private readonly Dictionary<string, int> _levels = new Dictionary<string, int>();

    public UpgradeService(EconomyService economy, GameSignals signals)
    {
        _economy = economy;
        _signals = signals;
    }

    public int GetLevel(UpgradeDefinition definition)
    {
        if (definition == null || string.IsNullOrEmpty(definition.Id))
            return 0;

        if (_levels.TryGetValue(definition.Id, out int level))
            return level;

        return 0;
    }

    public int GetCost(UpgradeDefinition definition)
    {
        if (definition == null)
            return int.MaxValue;

        int level = GetLevel(definition);
        return definition.GetCostAtLevel(level);
    }

    public bool IsMaxLevel(UpgradeDefinition definition)
    {
        if (definition == null)
            return true;

        int level = GetLevel(definition);
        return definition.IsMaxLevel(level);
    }

    public bool TryPurchase(UpgradeDefinition definition)
    {
        if (definition == null)
            return false;

        if (IsMaxLevel(definition))
            return false;

        int cost = GetCost(definition);
        if (!_economy.TrySpend(cost))
            return false;

        int newLevel = GetLevel(definition) + 1;
        _levels[definition.Id] = newLevel;

        _signals.RaiseUpgradePurchased(definition, newLevel);
        return true;
    }
}