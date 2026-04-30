using System.Collections.Generic;

[System.Serializable]
public class InventoryItem
{
    public int InstanceId;
    public PlaceablePartDefinition PartDefinition;
    public List<ModifierDefinition> Modifiers = new List<ModifierDefinition>();

    public InventoryItem(int instanceId, PlaceablePartDefinition partDefinition)
    {
        InstanceId = instanceId;
        PartDefinition = partDefinition;
    }

    public InventoryItem(int instanceId, PlaceablePartDefinition partDefinition, List<ModifierDefinition> modifiers)
    {
        InstanceId = instanceId;
        PartDefinition = partDefinition;

        if (modifiers != null)
            Modifiers.AddRange(modifiers);
    }
}