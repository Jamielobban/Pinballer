using UnityEngine;

public class DrainZoneView : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        BallView ballView = other.GetComponent<BallView>();
        if (ballView == null)
            return;

        BallRuntimeData runtimeData = ballView.RuntimeData;
        if (runtimeData == null)
            return;

        bool handledByAbility = BallAbilityRunner.OnDrain(runtimeData);

        if (handledByAbility)
            return;

        GameBootstrap.Context.Loop.OnBallDrained(runtimeData);

        if (ballView.gameObject != null)
            Destroy(ballView.gameObject);
    }
}