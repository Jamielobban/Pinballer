using UnityEngine;

[CreateAssetMenu(menuName = "Game/Shop/Pack Definition")]
public class ShopPackDefinition : ScriptableObject
{
    public string Id;
    public string DisplayName;

    [Header("Cost")]
    public int Cost = 10;

    [Header("Choices")]
    public int ChoiceCount = 3;

    [Header("Offer Pool")]
    public PlaceablePartDefinition[] AvailableParts;
    public ModifierDefinition[] AvailableModifiers;
    public UpgradeDefinition[] AvailableUpgrades;

    [Header("Roll Settings")]
    [Range(0, 100)] public int UpgradeOfferChancePercent = 30;
    [Range(0, 100)] public int ModifierChancePercent = 70;
    [Range(0, 100)] public int ExtraModifierChancePercent = 20;
    public int ModifierCost = 5;
}