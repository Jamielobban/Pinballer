using UnityEngine;

public static class PlaceableHitResolver
{
    public static void ResolveBasicHit(
        PlaceableRuntimeData runtime,
        BallRuntimeData ballData,
        Vector2 hitPoint,
        int baseHitScore,
        bool alwaysAddsCombo)
    {
        if (GameBootstrap.Context == null)
            return;

        if (alwaysAddsCombo || HasComboOnHit(runtime))
        {
            GameBootstrap.Context.Score.RegisterComboHit();
        }

        if (baseHitScore > 0)
        {
            GameBootstrap.Context.Score.AddScore(baseHitScore);
        }

        ApplyModifierEffects(runtime, ballData, hitPoint);
    }

    private static bool HasComboOnHit(PlaceableRuntimeData runtime)
    {
        return runtime != null &&
               runtime.HasModifier(ModifierType.ComboOnHit);
    }

    private static void ApplyModifierEffects(
        PlaceableRuntimeData runtime,
        BallRuntimeData ballData,
        Vector2 hitPoint)
    {
        if (runtime == null)
            return;

        if (runtime.HasModifier(ModifierType.Explode))
        {
            Debug.Log("Explode modifier triggered at " + hitPoint);
        }

        if (runtime.HasModifier(ModifierType.Chain))
        {
            Debug.Log("Chain modifier triggered at " + hitPoint);
        }
    }
}