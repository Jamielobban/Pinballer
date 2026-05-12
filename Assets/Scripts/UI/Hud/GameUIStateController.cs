using UnityEngine;

public class GameUIStateController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject packOpeningPanel;
    [SerializeField] private GameObject boardEditPanel;
    //[SerializeField] private GameObject gameplayHud;
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
        Refresh();
    }

    private void OnDestroy()
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
        if (GameBootstrap.Context == null)
            return;

        GameState state = GameBootstrap.Context.StateMachine.CurrentState;

        Set(shopPanel, state == GameState.ShopBuild);
        Set(packOpeningPanel, state == GameState.PackOpening);
        Set(boardEditPanel, state == GameState.BoardEdit);

        bool gameplay =
            state == GameState.WaitingForBall ||
            state == GameState.BallLoaded ||
            state == GameState.BallLaunching ||
            state == GameState.BallInPlay ||
            state == GameState.ResolvingDrain;

        //Set(gameplayHud, gameplay);
        Set(gameOverPanel, state == GameState.GameOver);
    }

    private void Set(GameObject panel, bool active)
    {
        if (panel == null)
            return;

        if (panel.activeSelf != active)
            panel.SetActive(active);
    }
}