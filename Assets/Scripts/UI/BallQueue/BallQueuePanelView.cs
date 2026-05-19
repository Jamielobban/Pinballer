using System.Collections.Generic;
using UnityEngine;

public class BallQueuePanelView : MonoBehaviour
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

        GameBootstrap.Context.Signals.BallQueueChanged += Refresh;
    }

    private void Unsubscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.BallQueueChanged -= Refresh;
    }

    private void Refresh()
    {
        Clear();

        if (GameBootstrap.Context == null)
            return;

        IReadOnlyList<BallRuntimeData> reserveBalls =
            GameBootstrap.Context.BallReserve.GetReserveSnapshot();

        for (int i = 0; i < reserveBalls.Count; i++)
        {
            BallRuntimeData ball = reserveBalls[i];

            if (ball == null)
                continue;

            BallQueueSlotView slot =
                Instantiate(slotPrefab, slotParent);

            slot.Setup(this, i, ball.Definition, 1);

            _spawnedSlots.Add(slot);

            UIAppearTween appear = slot.GetComponent<UIAppearTween>();

            if (appear != null)
                appear.Play(i * 0.04f);
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

    public void OnSlotClicked(int index)
    {
        // Current round queue is not editable yet.
    }
}