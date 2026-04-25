using UnityEngine;

public class ShopPanelView : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;

    private void Start()
    {
        if (panelRoot == null)
            panelRoot = gameObject;

        Subscribe();
        Refresh();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
    }

    private void Unsubscribe()
    {
        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (panelRoot == null || GameBootstrap.Context == null)
            return;

        panelRoot.SetActive(GameBootstrap.Context.StateMachine.IsInState(GameState.ShopBuild));
    }
}