using System.Collections.Generic;
using UnityEngine;

public class BallCollectionPanelView : MonoBehaviour
{
    [SerializeField] private Transform slotParent;
    [SerializeField] private BallQueueSlotView slotPrefab;

    private readonly List<BallQueueSlotView> _spawnedSlots = new();

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

        GameBootstrap.Context.BallInventory.OnChanged += Refresh;
        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
    }

    private void Unsubscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.BallInventory.OnChanged -= Refresh;
        GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        Refresh();
    }

    private void Refresh()
    {
        Clear();

        if (GameBootstrap.Context == null)
            return;

        IReadOnlyList<BallRuntimeData> ownedBalls =
            GameBootstrap.Context.BallInventory.OwnedBalls;

        Dictionary<BallDefinition, int> counts = new();

        for (int i = 0; i < ownedBalls.Count; i++)
        {
            if (ownedBalls[i] == null || ownedBalls[i].Definition == null)
                continue;

            BallDefinition definition = ownedBalls[i].Definition;

            if (!counts.ContainsKey(definition))
                counts.Add(definition, 0);

            counts[definition]++;
        }

        int visualIndex = 0;

        foreach (KeyValuePair<BallDefinition, int> pair in counts)
        {
            BallQueueSlotView slot =
                Instantiate(slotPrefab, slotParent);

            slot.Setup(null, visualIndex, pair.Key, pair.Value);

            _spawnedSlots.Add(slot);

            UIAppearTween appear = slot.GetComponent<UIAppearTween>();

            if (appear != null)
                appear.Play(visualIndex * 0.02f);

            visualIndex++;
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