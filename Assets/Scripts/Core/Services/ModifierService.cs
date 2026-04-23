using System.Collections.Generic;

public class ModifierService
{
    private readonly GameSignals _signals;
    private readonly List<ModifierDefinition> _activeModifiers = new List<ModifierDefinition>();

    public ModifierService(GameSignals signals)
    {
        _signals = signals;
    }

    public IReadOnlyList<ModifierDefinition> ActiveModifiers => _activeModifiers;

    public void AddModifier(ModifierDefinition modifier)
    {
        if (modifier == null)
            return;

        _activeModifiers.Add(modifier);
        _signals.RaiseModifierApplied(modifier);
    }

    public void ClearAll()
    {
        _activeModifiers.Clear();
    }
}