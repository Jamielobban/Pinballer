using System;

public class RNGService
{
    private System.Random _random;

    public int Seed { get; private set; }

    public RNGService(int seed)
    {
        SetSeed(seed);
    }

    public void SetSeed(int seed)
    {
        Seed = seed;
        _random = new System.Random(seed);
    }

    public int Range(int minInclusive, int maxExclusive)
    {
        return _random.Next(minInclusive, maxExclusive);
    }

    public float Value()
    {
        return (float)_random.NextDouble();
    }

    public bool RollChance(int percent)
    {
        return Range(0, 100) < percent;
    }
}