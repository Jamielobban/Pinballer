using UnityEngine;

public class ScoreService
{
    private readonly GameSession _session;
    private readonly GameSignals _signals;

    private float _comboMultiplier = 1f;
    private float _comboTimer;
    private float _comboDuration = 3f;

    private float _comboGainPerHit = 0.1f;
    private float _maxComboMultiplier = 3f;

    public int TotalScore => _session.TotalScore;
    public int RoundScore => _session.RoundScore;
    public int TargetScore => _session.TargetScore;

    public float ComboMultiplier => _comboMultiplier;
    public float ComboTimeRemaining => _comboTimer;
    public float ComboDuration => _comboDuration;

    public ScoreService(GameSession session, GameSignals signals)
    {
        _session = session;
        _signals = signals;
    }

    public void StartRound(int targetScore)
    {
        _session.ResetRoundScore();
        _session.SetTargetScore(targetScore);

        ResetCombo();
        RaiseScoreChanged();
    }

    public void Tick(float deltaTime)
    {
        if (_comboMultiplier <= 1f)
            return;

        _comboTimer -= deltaTime;

        if (_comboTimer <= 0f)
        {
            ResetCombo();
            return;
        }

        RaiseComboChanged();
    }

    public void RegisterComboHit()
    {
        _comboMultiplier += _comboGainPerHit;
        _comboMultiplier = Mathf.Min(_comboMultiplier, _maxComboMultiplier);

        _comboTimer = _comboDuration;

        RaiseComboChanged();
    }

    public void AddScore(int baseAmount)
    {
        if (baseAmount <= 0)
            return;

        float globalMultiplier = GameBootstrap.Context.Stats.GetScoreMultiplier();

        float finalMultiplier = globalMultiplier * _comboMultiplier;

        int finalAmount = Mathf.Max(1, Mathf.RoundToInt(baseAmount * finalMultiplier));

        _session.AddScore(finalAmount);

        Debug.Log(
            $"SCORE | Base: {baseAmount} | Global x{globalMultiplier:0.00} | Combo x{_comboMultiplier:0.00} | Final: {finalAmount}"
        );

        RaiseScoreChanged();
    }

    public bool HasMetTarget()
    {
        return _session.RoundScore >= _session.TargetScore;
    }

    private void ResetCombo()
    {
        _comboMultiplier = 1f;
        _comboTimer = 0f;

        RaiseComboChanged();
    }

    private void RaiseScoreChanged()
    {
        _signals.RaiseScoreChanged(
            _session.TotalScore,
            _session.RoundScore,
            _session.TargetScore
        );
    }

    private void RaiseComboChanged()
    {
        _signals.RaiseComboChanged(
            _comboMultiplier,
            _comboTimer,
            _comboDuration
        );
    }
}