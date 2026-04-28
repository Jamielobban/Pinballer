using UnityEngine;

public class BoardEditPanelView : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private UIPanelTween inventoryTween;

    private void Start()
    {
        if (panelRoot == null)
            panelRoot = gameObject;

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
        bool show = GameBootstrap.Context.StateMachine.IsInState(GameState.BoardEdit);

        panelRoot.SetActive(show);

        if (inventoryTween != null)
        {
            if (show)
                inventoryTween.Show();
            else
                inventoryTween.Hide();
        }
    }
}