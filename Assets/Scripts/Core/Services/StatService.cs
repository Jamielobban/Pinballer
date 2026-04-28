using System.Collections.Generic;

public class StatService
{
    private readonly UpgradeService _upgrades;
    private readonly ModifierService _modifiers;
    private readonly List<UpgradeDefinition> _upgradeDefinitions;

    public StatService(
        UpgradeService upgrades,
        ModifierService modifiers,
        List<UpgradeDefinition> upgradeDefinitions)
    {
        _upgrades = upgrades;
        _modifiers = modifiers;
        _upgradeDefinitions = upgradeDefinitions;
    }

    public float GetStatValue(StatType statType, float baseValue)
    {
        float addTotal = 0f;
        float multiplyTotal = 1f;

        for (int i = 0; i < _upgradeDefinitions.Count; i++)
        {
            UpgradeDefinition definition = _upgradeDefinitions[i];
            if (definition == null || definition.AffectedStat != statType)
                continue;

            int level = _upgrades.GetLevel(definition);
            if (level <= 0)
                continue;

            addTotal += definition.FlatIncrease * level;
            multiplyTotal += definition.PercentIncrease * level;
        }

        IReadOnlyList<ModifierDefinition> modifiers = _modifiers.ActiveModifiers;
        for (int i = 0; i < modifiers.Count; i++)
        {
            ModifierDefinition modifier = modifiers[i];
            if (modifier == null || modifier.AffectedStat != statType)
                continue;

            switch (modifier.Operation)
            {
                case ModifierOperation.Add:
                    addTotal += modifier.Value;
                    break;

                case ModifierOperation.Multiply:
                    multiplyTotal *= modifier.Value;
                    break;
            }
        }

        float result = (baseValue + addTotal) * multiplyTotal;
        return result;
    }

    public int GetIntStat(StatType statType, int baseValue)
    {
        return UnityEngine.Mathf.RoundToInt(GetStatValue(statType, baseValue));
    }

    public float GetMoneyPerHit()
    {
        return GetStatValue(StatType.MoneyPerHit, 1f);
    }

    public float GetBumperHitValue()
    {
        return GetStatValue(StatType.BumperHitValue, 2f);
    }

    public float GetTargetHitValue()
    {
        return GetStatValue(StatType.TargetHitValue, 3f);
    }

    public float GetBallSize()
    {
        return GetStatValue(StatType.BallSize, 1f);
    }

    public float GetLaunchPower()
    {
        return GetStatValue(StatType.LaunchPower, 1f);
    }

    public float GetAutoLoadSpeed()
    {
        return GetStatValue(StatType.AutoLoadSpeed, 1f);
    }

    public int GetMultiballCount()
    {
        return GetIntStat(StatType.MultiballCount, 1);
    }

    public float GetBumperForce()
    {
        return GetStatValue(StatType.BumperForce, 1f);
    }

    public float GetFlipperPower()
    {
        return GetStatValue(StatType.FlipperPower, 1f);
    }

    public float GetBallSaveDuration()
    {
        return GetStatValue(StatType.BallSaveDuration, 0f);
    }

    public int GetMaxBallReserve()
    {
        return GetIntStat(StatType.MaxBallReserve, 3);
    }

    public int GetBallsPerRound()
    {
        return GetIntStat(StatType.BallsPerRound, 3);
    }

    
}