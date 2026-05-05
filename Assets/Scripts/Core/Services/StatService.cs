using System.Collections.Generic;
using UnityEngine;

public class StatService
{
    private readonly UpgradeService _upgrades;
    private readonly List<UpgradeDefinition> _upgradeDefinitions;

    public StatService(
        UpgradeService upgrades,
        ModifierService modifiers,
        List<UpgradeDefinition> upgradeDefinitions)
    {
        _upgrades = upgrades;
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

            if (definition.PercentIncrease != 0f)
                multiplyTotal += definition.PercentIncrease * level;
        }

        return (baseValue + addTotal) * multiplyTotal;
    }

    public int GetIntStat(StatType statType, int baseValue)
    {
        return Mathf.RoundToInt(GetStatValue(statType, baseValue));
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

    public float GetBumperForce()
    {
        return GetStatValue(StatType.BumperForce, 1f);
    }

    public float GetFlipperPower()
    {
        return GetStatValue(StatType.FlipperPower, 1f);
    }

    public int GetBallsPerRound()
    {
        return GetIntStat(StatType.BallsPerRound, 3);
    }
    public float GetScoreMultiplier()
    {
        return GetStatValue(StatType.ScoreMultiplier, 1f);
    }
}