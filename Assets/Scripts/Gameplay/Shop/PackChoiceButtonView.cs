using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PackChoiceButtonView : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    private PackOpeningPanelView _panel;
    private PackOpeningManager _packOpeningManager;
    private int _choiceIndex = -1;
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

    public void Bind(PackOpeningPanelView panel, PackOpeningManager packOpeningManager, int choiceIndex)
    {
        _panel = panel;
        _packOpeningManager = packOpeningManager;
        _choiceIndex = choiceIndex;
    }

    public void Refresh()
    {
        if (button == null || label == null || _packOpeningManager == null)
            return;

        if (_choiceIndex < 0 || _choiceIndex >= _packOpeningManager.CurrentChoices.Count)
        {
            label.text = "Empty";
            SetInteractable(false);
            return;
        }

        ShopOffer choice = _packOpeningManager.CurrentChoices[_choiceIndex];

        if (choice == null)
        {
            label.text = "Missing Choice";
            SetInteractable(false);
            return;
        }

        label.text =
            $"{choice.GetDisplayName()}\n" +
            $"Type: {choice.OfferType}\n" +
            $"Mods: {GetModifierText(choice)}";

        SetInteractable(true);
    }

    private void OnClicked()
    {
        if (_panel == null)
            return;

        _panel.HandleChoiceClicked(this);
    }

    public void ExecuteClick()
    {
        if (_packOpeningManager == null)
            return;

        _packOpeningManager.ChooseReward(_choiceIndex);
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

    private string GetModifierText(ShopOffer choice)
    {
        if (choice == null || choice.Modifiers == null || choice.Modifiers.Count == 0)
            return "None";

        string text = "";

        for (int i = 0; i < choice.Modifiers.Count; i++)
        {
            if (choice.Modifiers[i] == null)
                continue;

            if (!string.IsNullOrEmpty(text))
                text += ", ";

            text += choice.Modifiers[i].DisplayName;
        }

        return string.IsNullOrEmpty(text) ? "None" : text;
    }
}