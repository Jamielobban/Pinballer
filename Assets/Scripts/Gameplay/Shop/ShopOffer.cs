using System.Collections.Generic;
public enum ShopOfferType
{
    Placeable,
    Upgrade
}

[System.Serializable]
public class ShopOffer
{
    public ShopOfferType OfferType;

    public PlaceablePartDefinition PartDefinition;
    public List<ModifierDefinition> Modifiers = new List<ModifierDefinition>();

    public UpgradeDefinition UpgradeDefinition;

    public int Cost;

    public string GetDisplayName()
    {
        if (OfferType == ShopOfferType.Upgrade)
            return UpgradeDefinition != null ? UpgradeDefinition.DisplayName : "Missing Upgrade";

        if (PartDefinition == null)
            return "Missing Item";

        if (Modifiers == null || Modifiers.Count == 0)
            return PartDefinition.DisplayName;

        return Modifiers[0].DisplayName + " " + PartDefinition.DisplayName;
    }
}