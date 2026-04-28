using System.Collections.Generic;
using UnityEngine;

public class InventoryPanelView : MonoBehaviour
{
    [SerializeField] private Transform slotParent;
    [SerializeField] private InventorySlotButton slotPrefab;

    private readonly List<InventorySlotButton> _spawnedSlots = new List<InventorySlotButton>();

    private void Start()
    {
        Subscribe();
        Refresh();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.InventoryChanged += Refresh;
        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
    }

    private void Unsubscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.InventoryChanged -= Refresh;
        GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        Refresh();
    }

    private void Refresh()
    {
        Clear();

        if (GameBootstrap.Context == null || slotParent == null || slotPrefab == null)
            return;

        if (!GameBootstrap.Context.StateMachine.IsInState(GameState.BoardEdit))
            return;

        IReadOnlyList<InventoryItem> items = GameBootstrap.Context.Inventory.Items;

        for (int i = 0; i < items.Count; i++)
        {
            InventorySlotButton slot = Instantiate(slotPrefab, slotParent);
            slot.Setup(items[i]);
            _spawnedSlots.Add(slot);
        }
    }

    private void Clear()
    {
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            if (_spawnedSlots[i] != null)
                Destroy(_spawnedSlots[i].gameObject);
        }

        _spawnedSlots.Clear();
    }
}