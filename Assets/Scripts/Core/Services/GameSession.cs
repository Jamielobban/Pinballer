public class GameSession
{
    public int Money { get; private set; }
    public int BallsInReserve { get; private set; }
    public int ActiveBallCount { get; private set; }
    public GameState CurrentState { get; private set; }

    public void SetMoney(int value)
    {
        Money = value;
    }

    public void AddMoney(int amount)
    {
        Money += amount;
    }

    public void SpendMoney(int amount)
    {
        Money -= amount;
        if (Money < 0)
            Money = 0;
    }

    public void SetReserve(int value)
    {
        BallsInReserve = value;
        if (BallsInReserve < 0)
            BallsInReserve = 0;
    }

    public void AddReserve(int amount)
    {
        BallsInReserve += amount;
        if (BallsInReserve < 0)
            BallsInReserve = 0;
    }

    public void ConsumeReserve(int amount)
    {
        BallsInReserve -= amount;
        if (BallsInReserve < 0)
            BallsInReserve = 0;
    }

    public void SetActiveBallCount(int count)
    {
        ActiveBallCount = count;
        if (ActiveBallCount < 0)
            ActiveBallCount = 0;
    }

    public void IncrementActiveBalls()
    {
        ActiveBallCount++;
    }

    public void DecrementActiveBalls()
    {
        ActiveBallCount--;
        if (ActiveBallCount < 0)
            ActiveBallCount = 0;
    }

    public void SetState(GameState state)
    {
        CurrentState = state;
    }
}