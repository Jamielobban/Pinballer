using UnityEngine;

public abstract class PlaceableModifierEffect : ScriptableObject
{
    public virtual void OnHit(
        PlaceableRuntimeData placeable,
        BallRuntimeData ball,
        HitEventData hitData)
    {
    }

    public virtual void OnPayout(
        PlaceableRuntimeData placeable,
        BallRuntimeData ball,
        HitEventData hitData)
    {
    }
}