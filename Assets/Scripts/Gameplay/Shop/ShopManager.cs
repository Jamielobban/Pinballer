using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Offer Pool")]
    [SerializeField] private PlaceablePartDefinition[] availableParts;
    [SerializeField] private ModifierDefinition[] availableModifiers;
    [SerializeField] private UpgradeDefinition[] availableUpgrades;

    [Header("Shop Settings")]
    [SerializeField] private int offerCount = 3;
    [SerializeField] private int upgradeOfferChancePercent = 30;
    [SerializeField] private int modifierChancePercent = 70;
    [SerializeField] private int extraModifierChancePercent = 20;
    [SerializeField] private int modifierCost = 5;

    [Header("Reroll")]
    [SerializeField] private int baseRerollCost = 5;
    [SerializeField] private int rerollCostIncrease = 3;

    private int _rerollsThisShop;

    private readonly List<ShopOffer> _currentOffers = new List<ShopOffer>();
    private bool _hasRolledForCurrentShop;

    public IReadOnlyList<ShopOffer> CurrentOffers => _currentOffers;

    public event System.Action OffersChanged;

    private void Start()
    {
        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;

        if (GameBootstrap.Context.StateMachine.IsInState(GameState.ShopBuild))
            EnterShop();
    }

    private void OnDestroy()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.ShopBuild)
        {
            EnterShop();
            return;
        }
        if (state == GameState.BoardEdit)
            return;

        _hasRolledForCurrentShop = false;
    }

    private void EnterShop()
    {
        if (_hasRolledForCurrentShop)
            return;

        _hasRolledForCurrentShop = true;
        _rerollsThisShop = 0;

        RollOffers();
    }
    public int GetCurrentRerollCost()
    {
        return baseRerollCost + (_rerollsThisShop * rerollCostIncrease);
    }
    public void RollOffers()
    {
        _currentOffers.Clear();

        if (!HasAnyOfferPool())
        {
            Debug.LogWarning("ShopManager has no available parts or upgrades.");
            OffersChanged?.Invoke();
            return;
        }

        for (int i = 0; i < offerCount; i++)
        {
            ShopOffer offer = GenerateOffer();

            if (offer != null)
                _currentOffers.Add(offer);
        }

        DebugLogOffers();
        OffersChanged?.Invoke();
    }

    private bool HasAnyOfferPool()
    {
        bool hasParts = availableParts != null && availableParts.Length > 0;
        bool hasUpgrades = availableUpgrades != null && availableUpgrades.Length > 0;

        return hasParts || hasUpgrades;
    }

    private ShopOffer GenerateOffer()
    {
        bool canRollUpgrade = availableUpgrades != null && availableUpgrades.Length > 0;
        bool canRollPart = availableParts != null && availableParts.Length > 0;

        if (!canRollUpgrade && !canRollPart)
            return null;

        bool rollUpgrade =
            canRollUpgrade &&
            Random.Range(0, 100) < upgradeOfferChancePercent;

        if (rollUpgrade || !canRollPart)
            return GenerateUpgradeOffer();

        return GeneratePlaceableOffer();
    }

    private ShopOffer GeneratePlaceableOffer()
    {
        PlaceablePartDefinition part = GetRandomPart();

        if (part == null)
            return null;

        ShopOffer offer = new ShopOffer
        {
            OfferType = ShopOfferType.Placeable,
            PartDefinition = part,
            Cost = part.BaseCost
        };

        TryAddRandomModifier(offer, modifierChancePercent);
        TryAddRandomModifier(offer, extraModifierChancePercent);

        return offer;
    }

    private ShopOffer GenerateUpgradeOffer()
    {
        UpgradeDefinition upgrade = GetRandomUpgrade();

        if (upgrade == null)
            return null;

        int cost = GameBootstrap.Context.Upgrades.GetCost(upgrade);

        return new ShopOffer
        {
            OfferType = ShopOfferType.Upgrade,
            UpgradeDefinition = upgrade,
            Cost = cost
        };
    }

    private PlaceablePartDefinition GetRandomPart()
    {
        if (availableParts == null || availableParts.Length == 0)
            return null;

        return availableParts[Random.Range(0, availableParts.Length)];
    }

    private UpgradeDefinition GetRandomUpgrade()
    {
        if (availableUpgrades == null || availableUpgrades.Length == 0)
            return null;

        return availableUpgrades[Random.Range(0, availableUpgrades.Length)];
    }

    private void TryAddRandomModifier(ShopOffer offer, int chancePercent)
    {
        if (offer == null || offer.PartDefinition == null)
            return;

        if (availableModifiers == null || availableModifiers.Length == 0)
            return;

        if (Random.Range(0, 100) >= chancePercent)
            return;

        ModifierDefinition modifier = GetRandomValidModifier(
            offer.PartDefinition,
            offer.Modifiers
        );

        if (modifier == null)
            return;

        offer.Modifiers.Add(modifier);
        offer.Cost += modifierCost;
    }

    private ModifierDefinition GetRandomValidModifier(
        PlaceablePartDefinition part,
        List<ModifierDefinition> existingModifiers)
    {
        List<ModifierDefinition> validModifiers = new List<ModifierDefinition>();

        for (int i = 0; i < availableModifiers.Length; i++)
        {
            ModifierDefinition modifier = availableModifiers[i];

            if (modifier == null)
                continue;

            if (existingModifiers != null && existingModifiers.Contains(modifier))
                continue;

            if (!part.CanUseModifier(modifier))
                continue;

            validModifiers.Add(modifier);
        }

        if (validModifiers.Count == 0)
            return null;

        return validModifiers[Random.Range(0, validModifiers.Count)];
    }

    public void BuyOffer(int index)
    {
        if (index < 0 || index >= _currentOffers.Count)
        {
            Debug.Log("Invalid shop offer index.");
            return;
        }

        ShopOffer offer = _currentOffers[index];

        if (offer == null)
            return;

        bool bought = false;

        if (offer.OfferType == ShopOfferType.Placeable)
            bought = BuyPlaceableOffer(offer);

        if (offer.OfferType == ShopOfferType.Upgrade)
            bought = BuyUpgradeOffer(offer);

        if (!bought)
            return;

        _currentOffers.RemoveAt(index);
        OffersChanged?.Invoke();

        Debug.Log("Bought: " + offer.GetDisplayName());
        DebugLogOffers();
    }

    private bool BuyPlaceableOffer(ShopOffer offer)
    {
        if (offer == null || offer.PartDefinition == null)
            return false;

        if (!GameBootstrap.Context.Economy.TrySpend(offer.Cost))
        {
            Debug.Log("Not enough money.");
            return false;
        }

        GameBootstrap.Context.Inventory.AddPart(
            offer.PartDefinition,
            offer.Modifiers
        );

        return true;
    }

    private bool BuyUpgradeOffer(ShopOffer offer)
    {
        if (offer == null || offer.UpgradeDefinition == null)
            return false;

        bool bought = GameBootstrap.Context.Upgrades.TryPurchase(offer.UpgradeDefinition);

        if (bought)
        {
            Debug.Log("Bought upgrade: " + offer.UpgradeDefinition.DisplayName);
            Debug.Log("Score Multiplier now: " + GameBootstrap.Context.Stats.GetScoreMultiplier());
        }

        return bought;
    }

    private void DebugLogOffers()
    {
        Debug.Log("=== SHOP OFFERS ===");

        if (_currentOffers.Count == 0)
        {
            Debug.Log("No offers.");
            return;
        }

        for (int i = 0; i < _currentOffers.Count; i++)
        {
            ShopOffer offer = _currentOffers[i];

            Debug.Log(
                (i + 1) + ": " +
                offer.GetDisplayName() +
                " | Type: " + offer.OfferType +
                " | Cost: " + offer.Cost +
                " | Mods: " + GetModifierNames(offer)
            );
        }
    }

    private string GetModifierNames(ShopOffer offer)
    {
        if (offer == null || offer.OfferType != ShopOfferType.Placeable)
            return "None";

        if (offer.Modifiers == null || offer.Modifiers.Count == 0)
            return "None";

        string result = "";

        for (int i = 0; i < offer.Modifiers.Count; i++)
        {
            ModifierDefinition modifier = offer.Modifiers[i];

            if (modifier == null)
                continue;

            if (result.Length > 0)
                result += ", ";

            result += modifier.DisplayName;
        }

        return result;
    }

    public void RerollOffer(int index)
    {
        if (index < 0 || index >= _currentOffers.Count)
        {
            Debug.Log("Invalid reroll index.");
            return;
        }

        int cost = GetCurrentRerollCost();

        if (!GameBootstrap.Context.Economy.TrySpend(cost))
        {
            Debug.Log("Not enough money to reroll.");
            return;
        }

        ShopOffer newOffer = GenerateUniqueOffer(index);

        if (newOffer == null)
        {
            Debug.Log("Could not generate unique offer.");
            return;
        }

        _currentOffers[index] = newOffer;
        _rerollsThisShop++;

        Debug.Log("Rerolled offer " + (index + 1) + " for " + cost);

        DebugLogOffers();
        OffersChanged?.Invoke();
    }

    private ShopOffer GenerateUniqueOffer(int replacingIndex)
    {
        const int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            ShopOffer candidate = GenerateOffer();

            if (candidate == null)
                continue;

            if (!DoesOfferDuplicateCurrent(candidate, replacingIndex))
                return candidate;
        }

        return GenerateOffer(); // fallback, avoids hard failure
    }

    private bool DoesOfferDuplicateCurrent(ShopOffer candidate, int ignoringIndex)
    {
        for (int i = 0; i < _currentOffers.Count; i++)
        {
            if (i == ignoringIndex)
                continue;

            ShopOffer existing = _currentOffers[i];

            if (AreOffersSame(candidate, existing))
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
}