using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopOfferButtonView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    [SerializeField] private Button rerollButton;
    [SerializeField] private int offerIndex;
    [SerializeField] private TMP_Text rerollLabel;

    private ShopManager _shopManager;
    private int _offerIndex = -1;

    private void Awake()
    {
        if (rerollButton != null)
            rerollButton.onClick.AddListener(OnRerollClicked);
    }

    private void OnRerollClicked()
    {
        if (_shopManager == null)
            return;

        _shopManager.RerollOffer(_offerIndex);
        rerollLabel.text = "Reroll: " + _shopManager.GetCurrentRerollCost();
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(BuyOffer);
    }

    public void Bind(ShopManager shopManager, int offerIndex)
    {
        _shopManager = shopManager;
        _offerIndex = offerIndex;

        Refresh();
    }

    public void Refresh()
    {
        if (button == null || label == null || _shopManager == null)
            return;

        if (_offerIndex < 0 || _offerIndex >= _shopManager.CurrentOffers.Count)
        {
            label.text = "Empty";
            button.interactable = false;
            return;
        }

        ShopOffer offer = _shopManager.CurrentOffers[_offerIndex];

        if (offer == null)
        {
            label.text = "Missing Offer";
            button.interactable = false;
            return;
        }

        bool canAfford = GameBootstrap.Context.Economy.CanAfford(offer.Cost);

        label.text =
            $"{offer.GetDisplayName()}\n" +
            $"Type: {offer.OfferType}\n" +
            $"Cost: {offer.Cost}\n" +
            $"Mods: {GetModifierText(offer)}";

        button.interactable = canAfford;
    }

    private void BuyOffer()
    {
        if (_shopManager == null)
            return;

        _shopManager.BuyOffer(_offerIndex);
    }

    private string GetModifierText(ShopOffer offer)
    {
        if (offer == null || offer.OfferType != ShopOfferType.Placeable)
            return "-";

        if (offer.Modifiers == null || offer.Modifiers.Count == 0)
            return "None";

        string text = "";

        for (int i = 0; i < offer.Modifiers.Count; i++)
        {
            if (offer.Modifiers[i] == null)
                continue;

            if (text.Length > 0)
                text += ", ";

            text += offer.Modifiers[i].DisplayName;
        }

        return text;
    }
}