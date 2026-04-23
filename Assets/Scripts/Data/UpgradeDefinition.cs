using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrades/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    [Header("Identity")]
    public string Id;
    public string DisplayName;
    [TextArea] public string Description;
    public UpgradeCategory Category;

    [Header("Targeting")]
    public MachinePartType TargetMachinePart = MachinePartType.None;
    public StatType AffectedStat = StatType.MoneyPerHit;

    [Header("Cost")]
    public int BaseCost = 10;
    public float CostGrowth = 1.5f;
    public int MaxLevel = 999;

    [Header("Effect")]
    public float FlatIncrease = 1f;
    public float PercentIncrease = 0f;

    public int GetCostAtLevel(int level)
    {
        return Mathf.RoundToInt(BaseCost * Mathf.Pow(CostGrowth, level));
    }

    public bool IsMaxLevel(int level)
    {
        return level >= MaxLevel;
    }
}