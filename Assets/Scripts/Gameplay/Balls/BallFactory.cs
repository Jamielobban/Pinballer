using UnityEngine;

public class BallFactory : MonoBehaviour
{
    [SerializeField] private BallView ballPrefab;

    private int _nextBallId = 1;

    public BallRuntimeData CreateBall(Vector3 position)
    {
        BallView ballView = Instantiate(ballPrefab, position, Quaternion.identity);

        BallRuntimeData runtimeData = new BallRuntimeData
        {
            BallId = _nextBallId++,
            BallObject = ballView.gameObject,
            IsLoaded = false,
            IsInPlay = false,
            SizeMultiplier = GameBootstrap.Context.Stats.GetBallSize(),
            SpeedMultiplier = 1f,
            ValueMultiplier = 1
        };

        ballView.Initialize(runtimeData);
        ballView.transform.localScale = Vector3.one * runtimeData.SizeMultiplier;

        return runtimeData;
    }
}