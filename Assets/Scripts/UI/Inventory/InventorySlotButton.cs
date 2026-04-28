using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image background;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;

    private InventoryItem _item;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(Select);
        else
            Debug.LogError("InventorySlotButton has no Button assigned.");
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(Select);
    }

    public void Setup(InventoryItem item)
    {
        _item = item;

        if (_item == null || _item.PartDefinition == null)
        {
            if (label != null)
                label.text = "Missing";

            if (button != null)
                button.interactable = false;

            return;
        }

        string displayName = _item.PartDefinition.DisplayName;
        if (string.IsNullOrWhiteSpace(displayName))
            displayName = _item.PartDefinition.name;

        if (label != null)
            label.text = displayName;

        if (button != null)
            button.interactable = true;

        SetSelected(false);

        Debug.Log("Inventory slot setup with: " + displayName);
    }

    private void Select()
    {
        Debug.Log("Inventory slot clicked.");

        if (_item == null)
        {
            Debug.LogError("Inventory slot has no item.");
            return;
        }

        if (_item.PartDefinition == null)
        {
            Debug.LogError("Inventory item has no PartDefinition.");
            return;
        }

        if (GameBootstrap.Context == null)
        {
            Debug.LogError("No GameBootstrap.Context.");
            return;
        }

        Debug.Log("Selected inventory item: " + _item.PartDefinition.DisplayName);

        GameBootstrap.Context.Inventory.SelectItem(_item);
        SetSelected(true);
    }

    private void SetSelected(bool selected)
    {
        if (background != null)
            background.color = selected ? selectedColor : normalColor;
    }
}