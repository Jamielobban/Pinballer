[System.Serializable]
public class ShopPackOffer
{
    public ShopPackDefinition PackDefinition;
    public int Cost;

    public string GetDisplayName()
    {
        if (PackDefinition == null)
            return "Missing Pack";

        return PackDefinition.DisplayName;
    }
}