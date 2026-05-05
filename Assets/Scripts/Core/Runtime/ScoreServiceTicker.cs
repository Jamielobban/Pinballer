using UnityEngine;

public class ScoreServiceTicker : MonoBehaviour
{
    private void Update()
    {
        if (GameBootstrap.Context == null || GameBootstrap.Context.Score == null)
            return;

        GameBootstrap.Context.Score.Tick(Time.deltaTime);
    }
}