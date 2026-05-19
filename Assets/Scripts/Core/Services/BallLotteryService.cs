using System.Collections.Generic;
using UnityEngine;

public class BallLotteryService
{
    private readonly BallInventoryService _ballInventory;
    private readonly RNGService _rng;

    public BallLotteryService(
        BallInventoryService ballInventory,
        RNGService rng)
    {
        _ballInventory = ballInventory;
        _rng = rng;
    }

    public List<BallRuntimeData> DrawBalls(int count)
    {
        IReadOnlyList<BallRuntimeData> owned = _ballInventory.OwnedBalls;

        if (owned.Count == 0)
        {
            Debug.LogWarning("No owned balls available for lottery draw.");
            return new List<BallRuntimeData>();
        }

        List<BallRuntimeData> pool = new List<BallRuntimeData>(owned);
        List<BallRuntimeData> result = new List<BallRuntimeData>();

        for (int i = 0; i < count; i++)
        {
            if (pool.Count == 0)
                break;

            int index = _rng.Range(0, pool.Count);

            BallRuntimeData drawnBall = pool[index];
            result.Add(drawnBall);

            pool.RemoveAt(index);
        }

        Debug.Log($"Lottery drew {result.Count} balls.");

        foreach (BallRuntimeData ball in result)
        {
            Debug.Log($"Drew ball: {ball.Definition.DisplayName}");
        }

        return result;
    }
}