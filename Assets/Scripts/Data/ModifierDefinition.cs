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

    public ModifierType Type;
    public float Value;
}