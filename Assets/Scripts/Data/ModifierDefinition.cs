using UnityEngine;

public enum ModifierType
{
    AddValue,
    MultiplyValue,
    Explode,
    Chain,
    ExtraBounce,

    ReduceHitsRequired,
    IncreaseHitsRequired,
    ComboOnHit
}

[CreateAssetMenu(menuName = "Game/Modifiers/Modifier Definition")]
public class ModifierDefinition : ScriptableObject
{
    public string Id;
    public string DisplayName;

    [TextArea]
    public string Description;

    [Header("Simple Stat Modifier")]
    public ModifierType Type;
    public float Value;

    [Header("Compatibility")]
    public PlaceablePartType[] AllowedPartTypes;

    [Header("Advanced Effects")]
    public PlaceableModifierEffect[] Effects;

    public bool CanApplyTo(PlaceablePartDefinition part)
    {
        if (part == null)
            return false;

        if (AllowedPartTypes == null || AllowedPartTypes.Length == 0)
            return true;

        for (int i = 0; i < AllowedPartTypes.Length; i++)
        {
            if (AllowedPartTypes[i] == part.PartType)
                return true;
        }

        return false;
    }
}