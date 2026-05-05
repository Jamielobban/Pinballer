using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlaceableRuntimeData
{
    public PlaceablePartDefinition Definition;
    public List<ModifierDefinition> Modifiers = new List<ModifierDefinition>();
    public GameObject Instance;

    public int CurrentHits;
    public int GetFinalValue()
    {
        if (Definition == null)
            return 1;

        float value = Definition.BaseValue;

        for (int i = 0; i < Modifiers.Count; i++)
        {
            ModifierDefinition mod = Modifiers[i];
            if (mod == null)
                continue;

            switch (mod.Type)
            {
                case ModifierType.AddValue:
                    value += mod.Value;
                    break;

                case ModifierType.MultiplyValue:
                    value *= mod.Value;
                    break;
            }
        }

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

    public bool HasModifier(ModifierType type)
    {
        for (int i = 0; i < Modifiers.Count; i++)
        {
            if (Modifiers[i] != null && Modifiers[i].Type == type)
                return true;
        }

        return false;
    }
    public int GetFinalHitsRequired()
    {
        if (Definition == null)
            return 1;

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

        return Mathf.Max(1, hits);
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
        return Mathf.Clamp01((float)CurrentHits / GetFinalHitsRequired());
    }
}