using UnityEngine;
public class ScoreService
{
    private readonly GameSession _session;
    private readonly GameSignals _signals;

    public int TotalScore => _session.TotalScore;
    public int RoundScore => _session.RoundScore;
    public int TargetScore => _session.TargetScore;

    public ScoreService(GameSession session, GameSignals signals)
    {
        _session = session;
        _signals = signals;
    }

    public void StartRound(int targetScore)
    {
        _session.ResetRoundScore();
        _session.SetTargetScore(targetScore);

        RaiseChanged();
    }

    public void AddScore(int baseAmount)
    {
        if (baseAmount <= 0)
            return;

        float multiplier = 1f;

        multiplier *= GameBootstrap.Context.Stats.GetScoreMultiplier();
        multiplier *= GetRoundMultiplier();

        int finalAmount = Mathf.Max(1, Mathf.RoundToInt(baseAmount * multiplier));

        _session.AddScore(finalAmount);

        _signals.RaiseScoreChanged(
            _session.TotalScore,
            _session.RoundScore,
            _session.TargetScore
        );
    }

    public bool HasMetTarget()
    {
        return _session.RoundScore >= _session.TargetScore;
    }

    private void RaiseChanged()
    {
        _signals.RaiseScoreChanged(
            _session.TotalScore,
            _session.RoundScore,
            _session.TargetScore
        );
    }
    private float GetRoundMultiplier()
    {
        int round = GameBootstrap.Context.Rounds.CurrentRound;

        return 1f + (round - 1) * 0.1f;
    }
    
}