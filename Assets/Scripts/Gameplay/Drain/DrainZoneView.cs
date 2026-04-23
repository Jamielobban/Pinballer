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

        GameBootstrap.Context.Loop.OnBallDrained(runtimeData);

        Destroy(ballView.gameObject);
    }
}