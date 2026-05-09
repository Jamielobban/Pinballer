using System.Collections.Generic;
using UnityEngine;

public class PackOpeningManager : MonoBehaviour
{
    private readonly List<ShopOffer> _currentChoices = new List<ShopOffer>();

    private ShopPackDefinition _currentPack;

    public IReadOnlyList<ShopOffer> CurrentChoices => _currentChoices;

    public event System.Action ChoicesChanged;

    public void OpenPack(ShopPackDefinition pack)
    {
        if (pack == null)
            return;

        _currentPack = pack;
        RollChoices(pack);

        GameBootstrap.Context.StateMachine.EnterState(GameState.PackOpening);
    }

    private void RollChoices(ShopPackDefinition pack)
    {
        _currentChoices.Clear();

        int count = Mathf.Max(1, pack.ChoiceCount);

        for (int i = 0; i < count; i++)
        {
            ShopOffer choice = GenerateUniqueChoice(pack);

            if (choice != null)
                _currentChoices.Add(choice);
        }

        DebugLogChoices();
        ChoicesChanged?.Invoke();
    }

    private ShopOffer GenerateUniqueChoice(ShopPackDefinition pack)
    {
        const int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            ShopOffer candidate = GenerateChoice(pack);

            if (candidate == null)
                continue;

            if (!DoesChoiceDuplicate(candidate))
                return candidate;
        }

        return GenerateChoice(pack);
    }

    private ShopOffer GenerateChoice(ShopPackDefinition pack)
    {
        bool canRollUpgrade = pack.AvailableUpgrades != null && pack.AvailableUpgrades.Length > 0;
        bool canRollPart = pack.AvailableParts != null && pack.AvailableParts.Length > 0;

        if (!canRollUpgrade && !canRollPart)
            return null;

        bool rollUpgrade =
            canRollUpgrade &&
            GameBootstrap.Context.RNG.RollChance(pack.UpgradeOfferChancePercent);

        if (rollUpgrade || !canRollPart)
            return GenerateUpgradeChoice(pack);

        return GeneratePlaceableChoice(pack);
    }

    private ShopOffer GeneratePlaceableChoice(ShopPackDefinition pack)
    {
        PlaceablePartDefinition part = pack.AvailableParts[
            GameBootstrap.Context.RNG.Range(0, pack.AvailableParts.Length)
        ];

        ShopOffer offer = new ShopOffer
        {
            OfferType = ShopOfferType.Placeable,
            PartDefinition = part,
            Cost = 0
        };

        TryAddRandomModifier(pack, offer, pack.ModifierChancePercent);
        TryAddRandomModifier(pack, offer, pack.ExtraModifierChancePercent);

        return offer;
    }

    private ShopOffer GenerateUpgradeChoice(ShopPackDefinition pack)
    {
        UpgradeDefinition upgrade = pack.AvailableUpgrades[
            GameBootstrap.Context.RNG.Range(0, pack.AvailableUpgrades.Length)
        ];

        return new ShopOffer
        {
            OfferType = ShopOfferType.Upgrade,
            UpgradeDefinition = upgrade,
            Cost = 0
        };
    }

    private void TryAddRandomModifier(
        ShopPackDefinition pack,
        ShopOffer offer,
        int chancePercent)
    {
        if (offer == null || offer.PartDefinition == null)
            return;

        if (pack.AvailableModifiers == null || pack.AvailableModifiers.Length == 0)
            return;

        if (!GameBootstrap.Context.RNG.RollChance(chancePercent))
            return;

        ModifierDefinition modifier = GetRandomValidModifier(pack, offer.PartDefinition, offer.Modifiers);

        if (modifier == null)
            return;

        offer.Modifiers.Add(modifier);
    }

    private ModifierDefinition GetRandomValidModifier(
        ShopPackDefinition pack,
        PlaceablePartDefinition part,
        List<ModifierDefinition> existingModifiers)
    {
        List<ModifierDefinition> valid = new List<ModifierDefinition>();

        for (int i = 0; i < pack.AvailableModifiers.Length; i++)
        {
            ModifierDefinition modifier = pack.AvailableModifiers[i];

            if (modifier == null)
                continue;

            if (existingModifiers != null && existingModifiers.Contains(modifier))
                continue;

            if (!part.CanUseModifier(modifier))
                continue;

            valid.Add(modifier);
        }

        if (valid.Count == 0)
            return null;

        return valid[
            GameBootstrap.Context.RNG.Range(0, valid.Count)
        ];
    }

    public void ChooseReward(int index)
    {
        if (index < 0 || index >= _currentChoices.Count)
            return;

        ShopOffer choice = _currentChoices[index];

        if (choice == null)
            return;

        GrantChoice(choice);

        _currentChoices.Clear();
        _currentPack = null;

        ChoicesChanged?.Invoke();

        GameBootstrap.Context.StateMachine.EnterState(GameState.ShopBuild);
    }

    private void GrantChoice(ShopOffer choice)
    {
        if (choice.OfferType == ShopOfferType.Placeable)
        {
            GameBootstrap.Context.Inventory.AddPart(
                choice.PartDefinition,
                choice.Modifiers
            );

            Debug.Log("Chose placeable reward: " + choice.GetDisplayName());
            return;
        }

        if (choice.OfferType == ShopOfferType.Upgrade)
        {
            GameBootstrap.Context.Upgrades.TryPurchase(choice.UpgradeDefinition);
            Debug.Log("Chose upgrade reward: " + choice.GetDisplayName());
        }
    }

    private bool DoesChoiceDuplicate(ShopOffer candidate)
    {
        for (int i = 0; i < _currentChoices.Count; i++)
        {
            if (AreOffersSame(candidate, _currentChoices[i]))
                return true;
        }

        return false;
    }

    private bool AreOffersSame(ShopOffer a, ShopOffer b)
    {
        if (a == null || b == null)
            return false;

        if (a.OfferType != b.OfferType)
            return false;

        if (a.OfferType == ShopOfferType.Upgrade)
            return a.UpgradeDefinition == b.UpgradeDefinition;

        if (a.PartDefinition != b.PartDefinition)
            return false;

        return HaveSameModifiers(a, b);
    }

    private bool HaveSameModifiers(ShopOffer a, ShopOffer b)
    {
        int aCount = a.Modifiers != null ? a.Modifiers.Count : 0;
        int bCount = b.Modifiers != null ? b.Modifiers.Count : 0;

        if (aCount != bCount)
            return false;

        for (int i = 0; i < aCount; i++)
        {
            ModifierDefinition modifier = a.Modifiers[i];

            if (modifier == null)
                continue;

            if (b.Modifiers == null || !b.Modifiers.Contains(modifier))
                return false;
        }

        return true;
    }

    private void DebugLogChoices()
    {
        Debug.Log("=== PACK CHOICES ===");

        for (int i = 0; i < _currentChoices.Count; i++)
        {
            Debug.Log((i + 1) + ": " + _currentChoices[i].GetDisplayName());
        }
    }
}