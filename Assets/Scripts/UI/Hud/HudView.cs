using TMPro;
using UnityEngine;

public class HudView : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private TMP_Text statsText;

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
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.MoneyChanged += OnMoneyChanged;
        GameBootstrap.Context.Signals.ScoreChanged += OnScoreChanged;
        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
        GameBootstrap.Context.Signals.ComboChanged += OnComboChanged;
    }

    private void Unsubscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.MoneyChanged -= OnMoneyChanged;
        GameBootstrap.Context.Signals.ScoreChanged -= OnScoreChanged;
        GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
        GameBootstrap.Context.Signals.ComboChanged -= OnComboChanged;
    }

    private void OnMoneyChanged(int money)
    {
        RefreshAll();
    }

    private void OnScoreChanged(int totalScore, int roundScore, int targetScore)
    {
        RefreshAll();
    }

    private void OnGameStateChanged(GameState state)
    {
        RefreshAll();
    }
    private void OnComboChanged(float multiplier, float timeRemaining, float duration)
    {
        RefreshAll();
    }

    private void RefreshAll()
    {
        if (GameBootstrap.Context == null)
            return;

        if (moneyText != null)
        {
            moneyText.text =
                $"Money: {GameBootstrap.Context.Economy.CurrentMoney}";
        }

        if (scoreText != null)
        {
            scoreText.text =
                $"Round Score: {GameBootstrap.Context.Score.RoundScore} / {GameBootstrap.Context.Score.TargetScore}\n" +
                $"Total Score: {GameBootstrap.Context.Score.TotalScore}";
        }

        if (stateText != null)
        {
            stateText.text =
                $"State: {GameBootstrap.Context.StateMachine.CurrentState}";
        }

        if (statsText != null)
        {
            statsText.text =
            $"Score Multiplier: x{GameBootstrap.Context.Stats.GetScoreMultiplier():0.00}\n" +
            $"Combo: x{GameBootstrap.Context.Score.ComboMultiplier:0.00}\n" +
            $"Combo Timer: {GameBootstrap.Context.Score.ComboTimeRemaining:0.00}s\n" +
            $"Balls/Round: {GameBootstrap.Context.Stats.GetBallsPerRound()}\n" +
            $"Launch Power: x{GameBootstrap.Context.Stats.GetLaunchPower():0.00}";
        }
    }
}