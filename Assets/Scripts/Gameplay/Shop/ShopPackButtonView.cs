using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPackButtonView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    private ShopManager _shopManager;
    private int _packIndex = -1;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(BuyPack);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(BuyPack);
    }

    public void Bind(ShopManager shopManager, int packIndex)
    {
        _shopManager = shopManager;
        _packIndex = packIndex;

        Refresh();
    }

    public void Refresh()
    {
        if (button == null || label == null || _shopManager == null)
            return;

        if (_packIndex < 0 || _packIndex >= _shopManager.CurrentPacks.Count)
        {
            label.text = "Empty";
            button.interactable = false;
            return;
        }

        ShopPackOffer offer = _shopManager.CurrentPacks[_packIndex];

        if (offer == null || offer.PackDefinition == null)
        {
            label.text = "Missing Pack";
            button.interactable = false;
            return;
        }

        bool canAfford = GameBootstrap.Context.Economy.CanAfford(offer.Cost);

        label.text =
            $"{offer.GetDisplayName()}\n" +
            $"Cost: {offer.Cost}\n" +
            $"Choices: {offer.PackDefinition.ChoiceCount}";

        button.interactable = canAfford;
    }

    private void BuyPack()
    {
        if (_shopManager == null)
            return;

        _shopManager.BuyPack(_packIndex);
    }
}