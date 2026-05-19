using UnityEngine;

public enum PlaceablePartType
{
    Bumper,
    Peg,
    Wall,
    Jackpot
}

[CreateAssetMenu(menuName = "Game/Build/Placeable Part Definition")]
public class PlaceablePartDefinition : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public PlaceablePartType PartType;

    [Header("Shop")]
    public int BaseCost = 10;

    [Header("Prefab")]
    public GameObject Prefab;

    [Header("Base Stats")]
    public int BaseValue = 1;
    public float Force = 10f;

    [Header("Default Modifiers")]
    public ModifierDefinition[] DefaultModifiers;

    [Header("Charge / Payout")]
    public int HitsRequired = 3;
    public int PayoutValue = 10;

    public bool CanUseModifier(ModifierDefinition modifier)
    {
        if (modifier == null)
            return false;

        return modifier.CanApplyTo(this);
    }
}