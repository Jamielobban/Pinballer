using UnityEngine;

public abstract class BallAbilityDefinition : ScriptableObject
{
    public virtual void OnSpawn(BallRuntimeData ball) { }

    public virtual void OnLaunch(BallRuntimeData ball) { }

    public virtual void OnHit(
        BallRuntimeData ball,
        HitEventData hitData)
    { }

    public virtual bool OnDrain(BallRuntimeData ball)
    {
        return false;
    }
}