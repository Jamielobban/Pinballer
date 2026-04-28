using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyShopPartButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private PlaceablePartDefinition partDefinition;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(Buy);
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
            button.onClick.RemoveListener(Buy);
    }

    private void Subscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.MoneyChanged += OnMoneyChanged;
        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
    }

    private void Unsubscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.MoneyChanged -= OnMoneyChanged;
        GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
    }

    private void Buy()
    {
        //Debug.Log("Start of buy");
        if (GameBootstrap.Context == null || partDefinition == null)
            return;

        if (!GameBootstrap.Context.StateMachine.IsInState(GameState.ShopBuild))
            return;

        if (!GameBootstrap.Context.Economy.TrySpend(partDefinition.BaseCost))
            return;

        GameBootstrap.Context.Inventory.AddPart(partDefinition);
        //Debug.Log("End of buy");
        Refresh();
    }

    private void OnMoneyChanged(int money)
    {
        Refresh();
    }

    private void OnGameStateChanged(GameState state)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (button == null || label == null || partDefinition == null || GameBootstrap.Context == null)
            return;

        bool isShop = GameBootstrap.Context.StateMachine.IsInState(GameState.ShopBuild);
        bool canAfford = GameBootstrap.Context.Economy.CanAfford(partDefinition.BaseCost);

        label.text =
            $"{partDefinition.DisplayName}\n" +
            $"Cost: {partDefinition.BaseCost}";

        button.interactable = isShop && canAfford;
    }
}