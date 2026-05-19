public static class BallAbilityRunner
{
    public static void OnSpawn(BallRuntimeData ball)
    {
        if (ball?.Definition?.Abilities == null)
            return;

        foreach (var ability in ball.Definition.Abilities)
        {
            if (ability == null)
                continue;

            ability.OnSpawn(ball);
        }
    }

    public static void OnLaunch(BallRuntimeData ball)
    {
        if (ball?.Definition?.Abilities == null)
            return;

        foreach (var ability in ball.Definition.Abilities)
        {
            if (ability == null)
                continue;

            ability.OnLaunch(ball);
        }
    }

    public static void OnHit(
        BallRuntimeData ball,
        HitEventData hitData)
    {
        if (ball == null)
            return;

        ball.HitCount++;

        if (ball.Definition?.Abilities == null)
            return;

        foreach (var ability in ball.Definition.Abilities)
        {
            if (ability == null)
                continue;

            ability.OnHit(ball, hitData);
        }
    }

    public static bool OnDrain(BallRuntimeData ball)
    {
        if (ball?.Definition?.Abilities == null)
            return false;

        foreach (var ability in ball.Definition.Abilities)
        {
            if (ability == null)
                continue;

            bool consumedDrain = ability.OnDrain(ball);

            if (consumedDrain)
                return true;
        }

        return false;
    }
}