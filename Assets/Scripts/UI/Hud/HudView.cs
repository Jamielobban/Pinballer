using TMPro;
using UnityEngine;

public class HudView : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text ballsText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text stateText;

    private bool _subscribed;

    private void Start()
    {
        Subscribe();
        RefreshAll();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (_subscribed)
            return;

        if (GameBootstrap.Context == null)
        {
            Debug.LogError("HudView could not subscribe because GameBootstrap.Context is null.");
            return;
        }

        GameBootstrap.Context.Signals.MoneyChanged += OnMoneyChanged;
        GameBootstrap.Context.Signals.BallReserveChanged += OnBallReserveChanged;
        GameBootstrap.Context.Signals.RoundChanged += OnRoundChanged;
        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;

        _subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_subscribed || GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.MoneyChanged -= OnMoneyChanged;
        GameBootstrap.Context.Signals.BallReserveChanged -= OnBallReserveChanged;
        GameBootstrap.Context.Signals.RoundChanged -= OnRoundChanged;
        GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;

        _subscribed = false;
    }

    private void RefreshAll()
    {
        if (GameBootstrap.Context == null)
            return;

        OnMoneyChanged(GameBootstrap.Context.Session.Money);
        OnBallReserveChanged(GameBootstrap.Context.BallReserve.CurrentReserve);
        OnRoundChanged(GameBootstrap.Context.Rounds.CurrentRound);
        OnGameStateChanged(GameBootstrap.Context.StateMachine.CurrentState);
    }

    private void OnMoneyChanged(int money)
    {
        if (moneyText != null)
            moneyText.text = $"Money: {money}";
    }

    private void OnBallReserveChanged(int balls)
    {
        if (ballsText != null)
            ballsText.text = $"Balls: {balls}";
    }

    private void OnRoundChanged(int round)
    {
        if (roundText != null)
            roundText.text = $"Round: {round}";
    }

    private void OnGameStateChanged(GameState state)
    {
        if (stateText != null)
            stateText.text = $"State: {state}";
    }
}