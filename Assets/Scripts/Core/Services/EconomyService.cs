public class EconomyService
{
    private readonly GameSession _session;
    private readonly GameSignals _signals;

    public EconomyService(GameSession session, GameSignals signals)
    {
        _session = session;
        _signals = signals;
    }

    public int CurrentMoney => _session.Money;

    public void SetStartingMoney(int amount)
    {
        _session.SetMoney(amount);
        _signals.RaiseMoneyChanged(_session.Money);
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        _session.AddMoney(amount);
        _signals.RaiseMoneyChanged(_session.Money);
    }

    public bool CanAfford(int amount)
    {
        return _session.Money >= amount;
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0)
            return true;

        if (!CanAfford(amount))
            return false;

        _session.SpendMoney(amount);
        _signals.RaiseMoneyChanged(_session.Money);
        return true;
    }
}