using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    [Header("Upgrade")]
    [SerializeField] private UpgradeDefinition upgradeDefinition;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(BuyUpgrade);
    }

    private void Start()
    {
        Subscribe();
        Refresh();
    }

    private void OnDestroy()
    {
        Unsubscribe();

        if (button != null)
            button.onClick.RemoveListener(BuyUpgrade);
    }

    private void Subscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.MoneyChanged += OnMoneyChanged;
        GameBootstrap.Context.Signals.UpgradePurchased += OnUpgradePurchased;
    }

    private void Unsubscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.MoneyChanged -= OnMoneyChanged;
        GameBootstrap.Context.Signals.UpgradePurchased -= OnUpgradePurchased;
    }

    private void BuyUpgrade()
    {
        if (GameBootstrap.Context == null || upgradeDefinition == null)
            return;

        GameBootstrap.Context.Upgrades.TryPurchase(upgradeDefinition);
        Refresh();
    }

    private void OnMoneyChanged(int money)
    {
        Refresh();
    }

    private void OnUpgradePurchased(UpgradeDefinition definition, int level)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (button == null || label == null || upgradeDefinition == null || GameBootstrap.Context == null)
            return;


        int level = GameBootstrap.Context.Upgrades.GetLevel(upgradeDefinition);
        int cost = GameBootstrap.Context.Upgrades.GetCost(upgradeDefinition);
        bool canAfford = GameBootstrap.Context.Economy.CanAfford(cost);

        label.text =
            $"{upgradeDefinition.DisplayName}\n" +
            $"Level: {level}\n" +
            $"Cost: {cost}";

        button.interactable = canAfford;
    }
}