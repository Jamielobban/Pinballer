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

    [Header("Allowed Modifiers")]
    public ModifierType[] AllowedModifierTypes;

    public bool CanUseModifier(ModifierDefinition modifier)
    {
        if (modifier == null)
            return false;

        if (AllowedModifierTypes == null || AllowedModifierTypes.Length == 0)
            return true;

        for (int i = 0; i < AllowedModifierTypes.Length; i++)
        {
            if (AllowedModifierTypes[i] == modifier.Type)
                return true;
        }

        return false;
    }
}