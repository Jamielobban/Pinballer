using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Offer Pool")]
    [SerializeField] private PlaceablePartDefinition[] availableParts;
    [SerializeField] private ModifierDefinition[] availableModifiers;

    [Header("Shop Settings")]
    [SerializeField] private int offerCount = 3;
    [SerializeField] private int modifierChancePercent = 70;
    [SerializeField] private int extraModifierChancePercent = 20;
    [SerializeField] private int modifierCost = 5;

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

        _hasRolledForCurrentShop = false;
    }

    private void EnterShop()
    {
        if (_hasRolledForCurrentShop)
            return;

        _hasRolledForCurrentShop = true;
        RollOffers();
    }

    public void RollOffers()
    {
        _currentOffers.Clear();

        if (availableParts == null || availableParts.Length == 0)
        {
            Debug.LogWarning("ShopManager has no available parts.");
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

    private ShopOffer GenerateOffer()
    {
        PlaceablePartDefinition part = GetRandomPart();

        if (part == null)
            return null;

        ShopOffer offer = new ShopOffer
        {
            PartDefinition = part,
            Cost = part.BaseCost
        };

        TryAddRandomModifier(offer, modifierChancePercent);
        TryAddRandomModifier(offer, extraModifierChancePercent);

        return offer;
    }

    private PlaceablePartDefinition GetRandomPart()
    {
        if (availableParts == null || availableParts.Length == 0)
            return null;

        return availableParts[Random.Range(0, availableParts.Length)];
    }

    private void TryAddRandomModifier(ShopOffer offer, int chancePercent)
    {
        if (offer == null || offer.PartDefinition == null)
            return;

        if (availableModifiers == null || availableModifiers.Length == 0)
            return;

        if (Random.Range(0, 100) >= chancePercent)
            return;

        ModifierDefinition modifier = GetRandomValidModifier(offer.PartDefinition, offer.Modifiers);

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

        if (offer == null || offer.PartDefinition == null)
            return;

        if (!GameBootstrap.Context.Economy.TrySpend(offer.Cost))
        {
            Debug.Log("Not enough money.");
            return;
        }

        GameBootstrap.Context.Inventory.AddPart(
            offer.PartDefinition,
            offer.Modifiers
        );

        _currentOffers.RemoveAt(index);
        OffersChanged?.Invoke();
        Debug.Log("Bought: " + offer.GetDisplayName());
        DebugLogOffers();
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
                i + 1 + ": " +
                offer.GetDisplayName() +
                " | Cost: " + offer.Cost +
                " | Mods: " + GetModifierNames(offer)
            );
        }
    }

    private string GetModifierNames(ShopOffer offer)
    {
        if (offer == null || offer.Modifiers == null || offer.Modifiers.Count == 0)
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
}