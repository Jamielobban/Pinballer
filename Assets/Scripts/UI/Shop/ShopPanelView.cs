using UnityEngine;

public class ShopPanelView : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private UIPanelTween panelTween;

    [Header("Shop UI")]
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ShopOfferButtonView[] offerButtons;

    private void Awake()
    {
        if (panelTween == null)
            panelTween = GetComponent<UIPanelTween>();

        if (shopManager == null)
            shopManager = FindFirstObjectByType<ShopManager>();
    }

    private void Start()
    {
        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;

        if (shopManager != null)
            shopManager.OffersChanged += RefreshOffers;

        BindButtons();
        Refresh();
    }

    private void OnDestroy()
    {
        if (GameBootstrap.Context != null)
            GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;

        if (shopManager != null)
            shopManager.OffersChanged -= RefreshOffers;
    }

    private void OnGameStateChanged(GameState state)
    {
        Refresh();
    }

    private void Refresh()
    {
        bool show = GameBootstrap.Context.StateMachine.IsInState(GameState.ShopBuild);

        if (show)
        {
            panelTween.Show();
            RefreshOffers(); // 🔥 important
        }
        else
        {
            panelTween.Hide();
        }
    }

    private void BindButtons()
    {
        if (offerButtons == null || shopManager == null)
            return;

        for (int i = 0; i < offerButtons.Length; i++)
        {
            if (offerButtons[i] != null)
                offerButtons[i].Bind(shopManager, i);
        }
    }

    private void RefreshOffers()
    {
        if (offerButtons == null)
            return;

        for (int i = 0; i < offerButtons.Length; i++)
        {
            if (offerButtons[i] != null)
                offerButtons[i].Refresh();
        }
    }
}