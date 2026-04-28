using UnityEngine;

public class BallFactory : MonoBehaviour
{
    [SerializeField] private BallDefinition defaultBallDefinition;

    private int _nextBallId = 1;

    public BallDefinition DefaultBallDefinition => defaultBallDefinition;

    public BallRuntimeData CreateBall(Vector3 position)
    {
        return CreateBall(position, defaultBallDefinition);
    }

    public BallRuntimeData CreateBall(Vector3 position, BallDefinition definition)
    {
        if (definition == null)
        {
            Debug.LogError("BallFactory: Missing BallDefinition.");
            return null;
        }

        if (definition.Prefab == null)
        {
            Debug.LogError($"BallFactory: BallDefinition {definition.name} has no prefab.");
            return null;
        }

        BallView ballView = Instantiate(definition.Prefab, position, Quaternion.identity);

        BallRuntimeData runtimeData = new BallRuntimeData
        {
            BallId = _nextBallId++,
            Definition = definition,
            BallObject = ballView.gameObject,
            IsLoaded = false,
            IsInPlay = false,
            SizeMultiplier = definition.SizeMultiplier,
            SpeedMultiplier = 1f,
            ValueMultiplier = definition.ValueMultiplier
        };

        ballView.Initialize(runtimeData);

        return runtimeData;
    }
}