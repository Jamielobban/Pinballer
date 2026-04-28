using UnityEngine;

public class ShopPanelView : MonoBehaviour
{
    [SerializeField] private UIPanelTween panelTween;

    private void Start()
    {
        if (panelTween == null)
            panelTween = GetComponent<UIPanelTween>();

        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
        Refresh();
    }

    private void OnDestroy()
    {
        if (GameBootstrap.Context != null)
            GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        Refresh();
    }

    private void Refresh()
    {
        bool show = GameBootstrap.Context.StateMachine.IsInState(GameState.ShopBuild);

        if (show)
            panelTween.Show();
        else
            panelTween.Hide();
    }
}