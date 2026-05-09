using UnityEngine;

public class PackOpeningPanelView : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private UIPanelTween panelTween;

    [Header("Pack Opening")]
    [SerializeField] private PackOpeningManager packOpeningManager;
    [SerializeField] private PackChoiceButtonView[] choiceButtons;

    private void Awake()
    {
        if (panelTween == null)
            panelTween = GetComponent<UIPanelTween>();

        if (packOpeningManager == null)
            packOpeningManager = FindFirstObjectByType<PackOpeningManager>();
    }

    private void Start()
    {
        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;

        if (packOpeningManager != null)
            packOpeningManager.ChoicesChanged += RefreshChoices;

        BindButtons();
        Refresh();
    }

    private void OnDestroy()
    {
        if (GameBootstrap.Context != null)
            GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;

        if (packOpeningManager != null)
            packOpeningManager.ChoicesChanged -= RefreshChoices;
    }

    private void OnGameStateChanged(GameState state)
    {
        Refresh();
    }

    private void BindButtons()
    {
        if (choiceButtons == null || packOpeningManager == null)
            return;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
                choiceButtons[i].Bind(packOpeningManager, i);
        }
    }

    private void Refresh()
    {
        bool show =
            GameBootstrap.Context != null &&
            GameBootstrap.Context.StateMachine.IsInState(GameState.PackOpening);

        if (show)
        {
            if (panelTween != null)
                panelTween.Show();

            RefreshChoices();
        }
        else
        {
            if (panelTween != null)
                panelTween.Hide();
        }
    }

    private void RefreshChoices()
    {
        if (choiceButtons == null)
            return;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
                choiceButtons[i].Refresh();
        }
    }
}