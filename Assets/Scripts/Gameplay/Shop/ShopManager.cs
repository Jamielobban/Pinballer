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

    private readonly List<ShopPackOffer> _currentPacks = new List<ShopPackOffer>();
    private bool _hasRolledForCurrentShop;

    public IReadOnlyList<ShopPackOffer> CurrentPacks => _currentPacks;

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

        // These are still the same shop phase.
        if (state == GameState.BoardEdit || state == GameState.PackOpening)
            return;

        // Only reset after leaving the whole shop/build phase,
        // like starting a round.
        _hasRolledForCurrentShop = false;
    }

    private void EnterShop()
    {
        if (_hasRolledForCurrentShop)
            return;

        _hasRolledForCurrentShop = true;
        RollPacks();
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
            ShopPackOffer offer = GeneratePackOffer();

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
        PacksChanged?.Invoke();

        Debug.Log("Bought pack: " + offer.GetDisplayName());

        packOpeningManager.OpenPack(packToOpen);
    }
    private void DebugLogPacks()
    {
        //Debug.Log("=== SHOP PACKS ===");

        //if (_currentPacks.Count == 0)
        //{
            //Debug.Log("No packs.");
            //return;
        //}

        //for (int i = 0; i < _currentPacks.Count; i++)
        //{
            //ShopPackOffer offer = _currentPacks[i];

            //Debug.Log(
                //(i + 1) + ": " +
                //offer.GetDisplayName() +
                //" | Cost: " + offer.Cost
            //);
        //}
    }
}