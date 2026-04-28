using System.Collections.Generic;

public class BallInventoryService
{
    private readonly GameSignals _signals;
    private readonly List<BallDefinition> _ownedBalls = new List<BallDefinition>();

    private int _nextBallIndex;

    public IReadOnlyList<BallDefinition> OwnedBalls => _ownedBalls;

    public BallInventoryService(GameSignals signals)
    {
        _signals = signals;
    }

    public void AddBall(BallDefinition ballDefinition)
    {
        if (ballDefinition == null)
            return;

        _ownedBalls.Add(ballDefinition);
        _signals.RaiseBallQueueChanged();
    }

    public void Clear()
    {
        _ownedBalls.Clear();
        _nextBallIndex = 0;
        _signals.RaiseBallQueueChanged();
    }

    public BallDefinition GetNextBall(BallDefinition fallbackBall)
    {
        if (_ownedBalls.Count == 0)
            return fallbackBall;

        if (_nextBallIndex >= _ownedBalls.Count)
            _nextBallIndex = 0;

        BallDefinition nextBall = _ownedBalls[_nextBallIndex];
        _nextBallIndex++;

        return nextBall != null ? nextBall : fallbackBall;
    }

    public void ResetQueue()
    {
        _nextBallIndex = 0;
    }

    public void SwapBalls(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= _ownedBalls.Count)
            return;

        if (indexB < 0 || indexB >= _ownedBalls.Count)
            return;

        if (indexA == indexB)
            return;

        BallDefinition temp = _ownedBalls[indexA];
        _ownedBalls[indexA] = _ownedBalls[indexB];
        _ownedBalls[indexB] = temp;

        _signals.RaiseBallQueueChanged();
    }
}