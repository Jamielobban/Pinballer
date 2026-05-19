using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotteryDrawController : MonoBehaviour
{
    [SerializeField] private float drawDelay = 0.25f;

    private Coroutine _drawRoutine;

    private void Start()
    {
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
    }

    private void Unsubscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state != GameState.LotteryDraw)
            return;

        StartLotteryDraw();
    }

    private void StartLotteryDraw()
    {
        if (_drawRoutine != null)
            StopCoroutine(_drawRoutine);

        _drawRoutine = StartCoroutine(DrawRoutine());
    }

    private IEnumerator DrawRoutine()
    {
        if (GameBootstrap.Context == null)
            yield break;

        int ballsThisRound =
            GameBootstrap.Context.Stats.GetBallsPerRound();

        List<BallRuntimeData> drawnBalls =
            GameBootstrap.Context.BallLottery.DrawBalls(ballsThisRound);

        GameBootstrap.Context.BallReserve.Clear();

        Debug.Log("=== LOTTERY DRAW START ===");

        for (int i = 0; i < drawnBalls.Count; i++)
        {
            BallRuntimeData ball = drawnBalls[i];

            if (ball == null || ball.Definition == null)
                continue;

            Debug.Log($"Lottery drew: {ball.Definition.DisplayName}");

            GameBootstrap.Context.BallReserve.AddToReserve(ball);

            yield return new WaitForSeconds(drawDelay);
        }

        Debug.Log("=== LOTTERY DRAW END ===");

        _drawRoutine = null;

        GameBootstrap.Context.Loop.FinishLotteryDraw();
    }
}