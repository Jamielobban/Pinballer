using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopRerollButtonView : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    private void Awake()
    {
        if (shopManager == null)
            shopManager = FindFirstObjectByType<ShopManager>();

        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(OnClicked);
    }

    private void Start()
    {
        if (shopManager != null)
            shopManager.PacksChanged += Refresh;

        Refresh();
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClicked);

        if (shopManager != null)
            shopManager.PacksChanged -= Refresh;
    }

    private void OnClicked()
    {
        if (shopManager == null)
            return;

        shopManager.ToggleRerollMode();
        Refresh();
    }

    private void Refresh()
    {
        if (shopManager == null || label == null)
            return;

        string prefix = shopManager.IsSelectingRerollTarget
            ? "Cancel Reroll"
            : "Reroll Pack";

        label.text = prefix + "\nCost: " + shopManager.GetCurrentRerollCost();
    }
}