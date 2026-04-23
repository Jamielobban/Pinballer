using UnityEngine;

[CreateAssetMenu(menuName = "Game/Modifiers/Modifier Definition")]
public class ModifierDefinition : ScriptableObject
{
    [Header("Identity")]
    public string Id;
    public string DisplayName;

    [Header("Scope")]
    public ModifierScope Scope = ModifierScope.Global;
    public MachinePartType TargetMachinePart = MachinePartType.None;

    [Header("Effect")]
    public StatType AffectedStat = StatType.MoneyPerHit;
    public ModifierOperation Operation = ModifierOperation.Add;
    public float Value = 1f;
}