[System.Serializable]
public class InventoryItem
{
    public int InstanceId;
    public PlaceablePartDefinition PartDefinition;

    public InventoryItem(int instanceId, PlaceablePartDefinition partDefinition)
    {
        InstanceId = instanceId;
        PartDefinition = partDefinition;
    }
}