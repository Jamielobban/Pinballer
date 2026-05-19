using UnityEngine;

[CreateAssetMenu(menuName = "Game/Balls/Abilities/Split Ball")]
public class SplitBallAbility : BallAbilityDefinition
{
    [SerializeField] private int hitsRequired = 8;
    [SerializeField] private int maxSplits = 1;
    [SerializeField] private Vector2 splitVelocityA = new Vector2(-4f, 6f);
    [SerializeField] private Vector2 splitVelocityB = new Vector2(4f, 6f);

    public override void OnSpawn(BallRuntimeData ball)
    {
        if (ball == null)
            return;

        ball.HitCount = 0;
        ball.RemainingSplits = maxSplits;
    }

    public override void OnHit(BallRuntimeData ball, HitEventData hitData)
    {
        if (ball == null)
            return;

        if (ball.RemainingSplits <= 0)
            return;

        if (ball.HitCount < hitsRequired)
            return;

        ball.RemainingSplits--;
        ball.HitCount = 0;

        Split(ball);
    }

    private void Split(BallRuntimeData sourceBall)
    {
        BallFactory factory = FindFirstObjectByType<BallFactory>();

        if (factory == null)
        {
            Debug.LogError("SplitBallAbility: No BallFactory found.");
            return;
        }

        if (sourceBall.BallObject == null)
            return;

        Vector3 spawnPosition = sourceBall.BallObject.transform.position;

        BallRuntimeData extraBall = factory.SpawnFromRuntime(
            spawnPosition,
            sourceBall
        );

        if (extraBall == null)
            return;

        extraBall.IsLoaded = false;
        extraBall.IsInPlay = true;
        extraBall.RemainingSplits = 0;

        BallView sourceView = sourceBall.BallObject.GetComponent<BallView>();
        BallView extraView = extraBall.BallObject.GetComponent<BallView>();

        if (sourceView != null)
            sourceView.LaunchWithVelocity(splitVelocityA);

        if (extraView != null)
            extraView.LaunchWithVelocity(splitVelocityB);

        if (GameBootstrap.Context != null)
            GameBootstrap.Context.BallLifecycle.RegisterSpawn(extraBall);

        Debug.Log("Split Ball split into an extra ball.");
    }
}