using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaceableRuntimeData
{
    public PlaceablePartDefinition Definition;
    public List<ModifierDefinition> Modifiers = new List<ModifierDefinition>();
    public GameObject Instance;

    public int CurrentHits;

    private const int MinimumHitsRequired = 3;

    public int GetFinalValue()
    {
        if (Definition == null)
            return 1;

        float value = Definition.BaseValue;
        float multiplier = 1f;

        for (int i = 0; i < Modifiers.Count; i++)
        {
            ModifierDefinition mod = Modifiers[i];

            if (mod == null)
                continue;

            if (mod.Type == ModifierType.AddValue)
                value += mod.Value;

            if (mod.Type == ModifierType.MultiplyValue)
                multiplier *= mod.Value;
        }

        value *= multiplier;

        return Mathf.Max(1, Mathf.RoundToInt(value));
    }

    public float GetFinalForce()
    {
        if (Definition == null)
            return 10f;

        float force = Definition.Force;

        for (int i = 0; i < Modifiers.Count; i++)
        {
            ModifierDefinition mod = Modifiers[i];

            if (mod == null)
                continue;

            if (mod.Type == ModifierType.ExtraBounce)
                force += mod.Value;
        }

        return Mathf.Max(0f, force);
    }

    public int GetFinalHitsRequired()
    {
        if (Definition == null)
            return MinimumHitsRequired;

        int hits = Definition.HitsRequired;

        for (int i = 0; i < Modifiers.Count; i++)
        {
            ModifierDefinition mod = Modifiers[i];

            if (mod == null)
                continue;

            if (mod.Type == ModifierType.ReduceHitsRequired)
                hits -= Mathf.RoundToInt(mod.Value);

            if (mod.Type == ModifierType.IncreaseHitsRequired)
                hits += Mathf.RoundToInt(mod.Value);
        }

        return Mathf.Max(MinimumHitsRequired, hits);
    }

    public int GetFinalPayoutValue()
    {
        if (Definition == null)
            return 1;

        float value = Definition.PayoutValue;
        float multiplier = 1f;

        for (int i = 0; i < Modifiers.Count; i++)
        {
            ModifierDefinition mod = Modifiers[i];

            if (mod == null)
                continue;

            if (mod.Type == ModifierType.AddValue)
                value += mod.Value;

            if (mod.Type == ModifierType.MultiplyValue)
                multiplier *= mod.Value;
        }

        value *= multiplier;

        return Mathf.Max(1, Mathf.RoundToInt(value));
    }

    public bool RegisterHitAndCheckPayout()
    {
        CurrentHits++;

        return CurrentHits >= GetFinalHitsRequired();
    }

    public void ResetCharge()
    {
        CurrentHits = 0;
    }

    public float GetChargePercent()
    {
        int hitsRequired = GetFinalHitsRequired();

        if (hitsRequired <= 0)
            return 0f;

        return Mathf.Clamp01((float)CurrentHits / hitsRequired);
    }

    public bool HasModifier(ModifierType type)
    {
        for (int i = 0; i < Modifiers.Count; i++)
        {
            if (Modifiers[i] != null && Modifiers[i].Type == type)
                return true;
        }

        return false;
    }

    public void TriggerModifierHitEffects(BallRuntimeData ball, HitEventData hitData)
    {
        if (Modifiers == null)
            return;

        for (int i = 0; i < Modifiers.Count; i++)
        {
            ModifierDefinition modifier = Modifiers[i];

            if (modifier == null || modifier.Effects == null)
                continue;

            for (int e = 0; e < modifier.Effects.Length; e++)
            {
                PlaceableModifierEffect effect = modifier.Effects[e];

                if (effect != null)
                    effect.OnHit(this, ball, hitData);
            }
        }
    }

    public void TriggerModifierPayoutEffects(BallRuntimeData ball, HitEventData hitData)
    {
        if (Modifiers == null)
            return;

        for (int i = 0; i < Modifiers.Count; i++)
        {
            ModifierDefinition modifier = Modifiers[i];

            if (modifier == null || modifier.Effects == null)
                continue;

            for (int e = 0; e < modifier.Effects.Length; e++)
            {
                PlaceableModifierEffect effect = modifier.Effects[e];

                if (effect != null)
                    effect.OnPayout(this, ball, hitData);
            }
        }
    }
}