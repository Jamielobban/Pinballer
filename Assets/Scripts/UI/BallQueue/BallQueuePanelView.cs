using System.Collections.Generic;
using UnityEngine;

public class BallQueuePanelView : MonoBehaviour
{
    [SerializeField] private Transform slotParent;
    [SerializeField] private BallQueueSlotView slotPrefab;

    private readonly List<BallQueueSlotView> _spawnedSlots = new List<BallQueueSlotView>();

    private int _selectedIndex = -1;

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
        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
    }

    private void Unsubscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.BallQueueChanged -= Refresh;
        GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        Refresh();
    }

    public void OnSlotClicked(int index)
    {
        if (GameBootstrap.Context == null)
            return;

        if (!CanEditQueue())
            return;

        if (_selectedIndex < 0)
        {
            _selectedIndex = index;
            UpdateSelectionVisuals();
            return;
        }

        if (_selectedIndex == index)
        {
            _selectedIndex = -1;
            UpdateSelectionVisuals();
            return;
        }

        GameBootstrap.Context.BallInventory.SwapBalls(_selectedIndex, index);
        _selectedIndex = -1;
        Refresh();
    }

    private void Refresh()
    {
        Clear();

        int ballsPerRound = GameBootstrap.Context.Stats.GetBallsPerRound();
        IReadOnlyList<BallDefinition> ownedBalls = GameBootstrap.Context.BallInventory.OwnedBalls;

        if (ownedBalls.Count == 0)
            return;

        for (int i = 0; i < ballsPerRound; i++)
        {
            BallDefinition ball = ownedBalls[i % ownedBalls.Count];

            BallQueueSlotView slot = Instantiate(slotPrefab, slotParent);
            slot.Setup(this, i, ball);
            _spawnedSlots.Add(slot);

            UIAppearTween appear = slot.GetComponent<UIAppearTween>();
            if (appear != null)
                appear.Play(i * 0.04f);
        }

        UpdateSelectionVisuals();
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

    private void UpdateSelectionVisuals()
    {
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            _spawnedSlots[i].SetSelected(i == _selectedIndex);
        }
    }

    private bool CanEditQueue()
    {
        return GameBootstrap.Context.StateMachine.IsInState(GameState.ShopBuild)
            || GameBootstrap.Context.StateMachine.IsInState(GameState.BoardEdit);
    }
}