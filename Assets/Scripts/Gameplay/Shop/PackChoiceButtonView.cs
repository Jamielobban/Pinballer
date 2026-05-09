using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PackChoiceButtonView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    private PackOpeningManager _packOpeningManager;
    private int _choiceIndex = -1;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(ChooseReward);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(ChooseReward);
    }

    public void Bind(PackOpeningManager packOpeningManager, int choiceIndex)
    {
        _packOpeningManager = packOpeningManager;
        _choiceIndex = choiceIndex;

        Refresh();
    }

    public void Refresh()
    {
        if (button == null || label == null || _packOpeningManager == null)
            return;

        if (_choiceIndex < 0 || _choiceIndex >= _packOpeningManager.CurrentChoices.Count)
        {
            label.text = "Empty";
            button.interactable = false;
            return;
        }

        ShopOffer choice = _packOpeningManager.CurrentChoices[_choiceIndex];

        if (choice == null)
        {
            label.text = "Missing Choice";
            button.interactable = false;
            return;
        }

        label.text =
            $"{choice.GetDisplayName()}\n" +
            $"Type: {choice.OfferType}\n" +
            $"Mods: {GetModifierText(choice)}";

        button.interactable = true;
    }

    private void ChooseReward()
    {
        if (_packOpeningManager == null)
            return;

        _packOpeningManager.ChooseReward(_choiceIndex);
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
            ModifierDefinition modifier = offer.Modifiers[i];

            if (modifier == null)
                continue;

            if (text.Length > 0)
                text += ", ";

            text += modifier.DisplayName;
        }

        return text;
    }
}