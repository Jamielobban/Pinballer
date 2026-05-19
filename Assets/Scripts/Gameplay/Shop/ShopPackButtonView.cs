using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPackButtonView : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    private ShopPanelView _panel;
    private ShopManager _shopManager;
    private int _packIndex = -1;
    private UIAnimatedCard _animatedCard;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        _animatedCard = GetComponent<UIAnimatedCard>();

        if (button != null)
            button.onClick.AddListener(OnClicked);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClicked);
    }

    public void Bind(ShopPanelView panel, ShopManager shopManager, int packIndex)
    {
        _panel = panel;
        _shopManager = shopManager;
        _packIndex = packIndex;
    }

    public void Refresh()
    {
        if (button == null || label == null || _shopManager == null)
            return;

        if (_packIndex < 0 || _packIndex >= _shopManager.CurrentPacks.Count)
        {
            label.text = "Empty";
            SetInteractable(false);
            return;
        }

        ShopPackOffer offer = _shopManager.CurrentPacks[_packIndex];

        if (offer == null || offer.PackDefinition == null)
        {
            label.text = "Missing Pack";
            SetInteractable(false);
            return;
        }

        bool canAfford = GameBootstrap.Context.Economy.CanAfford(offer.Cost);

        label.text =
            $"{offer.GetDisplayName()}\n" +
            $"Cost: {offer.Cost}\n" +
            $"Choices: {offer.PackDefinition.ChoiceCount}";

        if (_shopManager.IsSelectingRerollTarget)
            label.text += "\nSELECT TO REROLL";

        SetInteractable(canAfford);
    }

    private void OnClicked()
    {
        if (_panel == null)
            return;

        _panel.HandlePackClicked(this);
    }

    public void ExecuteClick()
    {
        if (_shopManager == null)
            return;

        _shopManager.BuyPack(_packIndex);
    }

    public void SetInteractable(bool interactable)
    {
        if (_animatedCard != null)
            _animatedCard.SetInteractable(interactable);
        else if (button != null)
            button.interactable = interactable;
    }

    public Tween PlayAppear(float delay)
    {
        if (_animatedCard == null)
            return null;

        return _animatedCard.PlayAppear(delay);
    }

    public Tween PlayDisappear(float delay = 0f)
    {
        if (_animatedCard == null)
            return null;

        return _animatedCard.PlayDisappear(delay);
    }
}