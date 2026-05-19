using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Pack Pool")]
    [SerializeField] private ShopPackDefinition[] availablePacks;

    [Header("References")]
    [SerializeField] private PackOpeningManager packOpeningManager;

    [Header("Shop Settings")]
    [SerializeField] private int packOfferCount = 3;

    [Header("Reroll")]
    [SerializeField] private int baseRerollCost = 5;
    [SerializeField] private int rerollCostIncrease = 3;

    private readonly List<ShopPackOffer> _currentPacks = new List<ShopPackOffer>();

    private bool _hasRolledForCurrentShop;
    private bool _isSelectingRerollTarget;
    private int _rerollsThisShop;

    public IReadOnlyList<ShopPackOffer> CurrentPacks => _currentPacks;
    public bool IsSelectingRerollTarget => _isSelectingRerollTarget;

    public event System.Action PacksChanged;

    private void Awake()
    {
        if (packOpeningManager == null)
            packOpeningManager = FindFirstObjectByType<PackOpeningManager>();
    }

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

        if (state == GameState.BoardEdit || state == GameState.PackOpening)
            return;

        _hasRolledForCurrentShop = false;
        _isSelectingRerollTarget = false;
    }

    private void EnterShop()
    {
        if (_hasRolledForCurrentShop)
            return;

        _hasRolledForCurrentShop = true;
        _rerollsThisShop = 0;
        _isSelectingRerollTarget = false;

        RollPacks();
    }

    public int GetCurrentRerollCost()
    {
        return baseRerollCost + (_rerollsThisShop * rerollCostIncrease);
    }

    public void ToggleRerollMode()
    {
        _isSelectingRerollTarget = !_isSelectingRerollTarget;

        Debug.Log(_isSelectingRerollTarget
            ? "Select a pack to reroll."
            : "Cancelled reroll mode.");

        PacksChanged?.Invoke();
    }

    public void CancelRerollMode()
    {
        if (!_isSelectingRerollTarget)
            return;

        _isSelectingRerollTarget = false;
        PacksChanged?.Invoke();
    }

    public void OnPackClicked(int index)
    {
        if (_isSelectingRerollTarget)
        {
            RerollPack(index);
            return;
        }

        BuyPack(index);
    }

    public void RollPacks()
    {
        _currentPacks.Clear();

        if (availablePacks == null || availablePacks.Length == 0)
        {
            Debug.LogWarning("ShopManager has no available packs.");
            PacksChanged?.Invoke();
            return;
        }

        for (int i = 0; i < packOfferCount; i++)
        {
            ShopPackOffer offer = GenerateUniquePackOffer(-1);

            if (offer != null)
                _currentPacks.Add(offer);
        }

        DebugLogPacks();
        PacksChanged?.Invoke();
    }

    private ShopPackOffer GeneratePackOffer()
    {
        ShopPackDefinition pack = availablePacks[
            GameBootstrap.Context.RNG.Range(0, availablePacks.Length)
        ];

        if (pack == null)
            return null;

        return new ShopPackOffer
        {
            PackDefinition = pack,
            Cost = pack.Cost
        };
    }

    private ShopPackOffer GenerateUniquePackOffer(int replacingIndex)
    {
        const int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            ShopPackOffer candidate = GeneratePackOffer();

            if (candidate == null)
                continue;

            if (!DoesPackDuplicateCurrent(candidate, replacingIndex))
                return candidate;
        }

        return GeneratePackOffer();
    }

    private bool DoesPackDuplicateCurrent(ShopPackOffer candidate, int replacingIndex)
    {
        for (int i = 0; i < _currentPacks.Count; i++)
        {
            if (i == replacingIndex)
                continue;

            ShopPackOffer existing = _currentPacks[i];

            if (existing == null || candidate == null)
                continue;

            if (existing.PackDefinition == candidate.PackDefinition)
                return true;
        }

        return false;
    }

    private void RerollPack(int index)
    {
        if (index < 0 || index >= _currentPacks.Count)
        {
            Debug.Log("Invalid pack reroll index.");
            return;
        }

        int cost = GetCurrentRerollCost();

        if (!GameBootstrap.Context.Economy.TrySpend(cost))
        {
            Debug.Log("Not enough money to reroll pack.");
            return;
        }

        ShopPackOffer newOffer = GenerateUniquePackOffer(index);

        if (newOffer == null)
        {
            Debug.Log("Could not generate replacement pack.");
            return;
        }

        _currentPacks[index] = newOffer;
        _rerollsThisShop++;
        _isSelectingRerollTarget = false;

        Debug.Log("Rerolled pack " + (index + 1) + " for " + cost);

        DebugLogPacks();
        PacksChanged?.Invoke();
    }

    public void BuyPack(int index)
    {
        if (index < 0 || index >= _currentPacks.Count)
        {
            Debug.Log("Invalid pack index.");
            return;
        }

        ShopPackOffer offer = _currentPacks[index];

        if (offer == null || offer.PackDefinition == null)
            return;

        if (packOpeningManager == null)
        {
            Debug.LogError("ShopManager has no PackOpeningManager.");
            return;
        }

        if (!GameBootstrap.Context.Economy.TrySpend(offer.Cost))
        {
            Debug.Log("Not enough money for pack.");
            return;
        }

        ShopPackDefinition packToOpen = offer.PackDefinition;

        _currentPacks.RemoveAt(index);
        _isSelectingRerollTarget = false;

        PacksChanged?.Invoke();

        Debug.Log("Bought pack: " + offer.GetDisplayName());

        packOpeningManager.OpenPack(packToOpen);
    }

    private void DebugLogPacks()
    {
        Debug.Log("=== SHOP PACKS ===");

        if (_currentPacks.Count == 0)
        {
            Debug.Log("No packs.");
            return;
        }

        for (int i = 0; i < _currentPacks.Count; i++)
        {
            ShopPackOffer offer = _currentPacks[i];

            Debug.Log(
                (i + 1) + ": " +
                offer.GetDisplayName() +
                " | Cost: " + offer.Cost
            );
        }
    }
}