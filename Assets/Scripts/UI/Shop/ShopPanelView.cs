using UnityEngine;

public class ShopPanelView : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ShopPackButtonView[] packButtons;

    private void Awake()
    {
        if (shopManager == null)
            shopManager = FindFirstObjectByType<ShopManager>();
    }

    private void Start()
    {
        if (shopManager != null)
            shopManager.PacksChanged += RefreshPacks;

        BindButtons();
        RefreshPacks();
    }

    private void OnDestroy()
    {
        if (shopManager != null)
            shopManager.PacksChanged -= RefreshPacks;
    }

    private void BindButtons()
    {
        if (packButtons == null || shopManager == null)
            return;

        for (int i = 0; i < packButtons.Length; i++)
        {
            if (packButtons[i] != null)
                packButtons[i].Bind(shopManager, i);
        }
    }

    private void RefreshPacks()
    {
        if (packButtons == null)
            return;

        for (int i = 0; i < packButtons.Length; i++)
        {
            if (packButtons[i] != null)
                packButtons[i].Refresh();
        }
    }
}