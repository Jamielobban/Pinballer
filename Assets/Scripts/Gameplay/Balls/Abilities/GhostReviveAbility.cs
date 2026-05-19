using UnityEngine;

[CreateAssetMenu(menuName = "Game/Balls/Abilities/Ghost Revive")]
public class GhostReviveAbility : BallAbilityDefinition
{
    [SerializeField] private int reviveCount = 1;

    [Header("Respawn")]
    [SerializeField] private Vector2 relaunchVelocity = new Vector2(0f, -8f);

    public override void OnSpawn(BallRuntimeData ball)
    {
        if (ball == null)
            return;

        ball.RemainingRevives = reviveCount;
    }

    public override bool OnDrain(BallRuntimeData ball)
    {
        if (ball == null)
            return false;

        if (ball.RemainingRevives <= 0)
            return false;

        BallFactory factory = FindFirstObjectByType<BallFactory>();
        BallSpawnPoints spawnPoints = FindFirstObjectByType<BallSpawnPoints>();

        if (factory == null)
        {
            Debug.LogError("GhostReviveAbility: No BallFactory found.");
            return false;
        }

        if (spawnPoints == null || spawnPoints.GhostRespawnPoint == null)
        {
            Debug.LogError("GhostReviveAbility: No GhostRespawnPoint found.");
            return false;
        }

        ball.RemainingRevives--;

        if (GameBootstrap.Context != null)
        {
            GameBootstrap.Context.BallLifecycle.DrainBall(ball);
        }

        if (ball.BallObject != null)
            Destroy(ball.BallObject);

        BallRuntimeData revivedBall = factory.SpawnFromRuntime(
            spawnPoints.GhostRespawnPoint.position,
            ball
        );

        if (revivedBall == null)
            return false;

        revivedBall.RemainingRevives = ball.RemainingRevives;
        revivedBall.HitCount = ball.HitCount;
        revivedBall.IsLoaded = false;
        revivedBall.IsInPlay = true;

        BallView view = revivedBall.BallObject.GetComponent<BallView>();

        if (view != null)
            view.LaunchWithVelocity(relaunchVelocity);

        if (GameBootstrap.Context != null)
            GameBootstrap.Context.BallLifecycle.RegisterSpawn(revivedBall);

        Debug.Log("Ghost Ball revived.");

        return true;
    }
}