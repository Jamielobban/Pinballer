using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallQueueSlotView : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Image icon;
    [SerializeField] private Image background;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private BallQueuePanelView _panel;
    private int _index;
    private BallDefinition _ballDefinition;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(OnClicked);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClicked);
    }

    public void Setup(
        BallQueuePanelView panel,
        int index,
        BallDefinition ballDefinition,
        int count = 1)
    {
        _panel = panel;
        _index = index;
        _ballDefinition = ballDefinition;

        string displayName = "Empty";

        if (_ballDefinition != null)
        {
            displayName = string.IsNullOrWhiteSpace(_ballDefinition.DisplayName)
                ? _ballDefinition.name
                : _ballDefinition.DisplayName;
        }

        if (label != null)
            label.text = displayName;

        if (countText != null)
        {
            bool showCount = count > 1;
            countText.gameObject.SetActive(showCount);
            countText.text = showCount ? $"x{count}" : "";
        }

        if (icon != null)
        {
            icon.enabled = _ballDefinition != null && _ballDefinition.Sprite != null;

            if (_ballDefinition != null && _ballDefinition.Sprite != null)
            {
                icon.sprite = _ballDefinition.Sprite;
                icon.color = _ballDefinition.Tint;
            }
        }

        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (background != null)
            background.color = selected ? selectedColor : normalColor;
    }

    private void OnClicked()
    {
        if (_panel == null)
            return;

        _panel.OnSlotClicked(_index);
    }
}