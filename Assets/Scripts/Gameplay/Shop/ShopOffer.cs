using System.Collections.Generic;

[System.Serializable]
public class ShopOffer
{
    public PlaceablePartDefinition PartDefinition;
    public List<ModifierDefinition> Modifiers = new List<ModifierDefinition>();
    public int Cost;

    public string GetDisplayName()
    {
        if (PartDefinition == null)
            return "Missing Item";

        if (Modifiers == null || Modifiers.Count == 0)
            return PartDefinition.DisplayName;

        return Modifiers[0].DisplayName + " " + PartDefinition.DisplayName;
    }
}