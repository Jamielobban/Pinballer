using System.Collections.Generic;

public class InventoryService
{
    private readonly GameSignals _signals;

    private readonly List<InventoryItem> _items = new List<InventoryItem>();
    private int _nextInstanceId = 1;

    public IReadOnlyList<InventoryItem> Items => _items;
    public InventoryItem SelectedItem { get; private set; }

    public InventoryService(GameSignals signals)
    {
        _signals = signals;
    }

    public void AddPart(PlaceablePartDefinition partDefinition)
    {
        if (partDefinition == null)
            return;

        InventoryItem item = new InventoryItem(_nextInstanceId, partDefinition);
        _nextInstanceId++;

        _items.Add(item);
        _signals.RaiseInventoryChanged();
    }

    public void SelectItem(InventoryItem item)
    {
        if (item == null)
            return;

        if (!_items.Contains(item))
            return;

        SelectedItem = item;
        _signals.RaiseInventoryItemSelected(item);
    }

    public void ClearSelection()
    {
        SelectedItem = null;
        _signals.RaiseInventoryItemSelected(null);
    }

    public void RemoveItem(InventoryItem item)
    {
        if (item == null)
            return;

        if (_items.Remove(item))
        {
            if (SelectedItem == item)
                SelectedItem = null;

            _signals.RaiseInventoryChanged();
            _signals.RaiseInventoryItemSelected(SelectedItem);
        }
    }
}