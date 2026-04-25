using UnityEngine;
using UnityEngine.UI;

public class StartRoundButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private bool _subscribed;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(StartRound);
    }

    private void Start()
    {
        Subscribe();
        Refresh();
    }

    private void OnDestroy()
    {
        Unsubscribe();

        if (button != null)
            button.onClick.RemoveListener(StartRound);
    }

    private void Subscribe()
    {
        if (_subscribed)
            return;

        if (GameBootstrap.Context == null)
        {
            Debug.LogError("StartRoundButton could not subscribe because GameBootstrap.Context is null.");
            return;
        }

        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
        _subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_subscribed || GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
        _subscribed = false;
    }

    private void StartRound()
    {
        if (GameBootstrap.Context == null)
            return;

        Debug.Log($"Button clicked. Current state: {GameBootstrap.Context.StateMachine.CurrentState}");

        if (!GameBootstrap.Context.StateMachine.IsInState(GameState.ShopBuild))
            return;

        GameBootstrap.Context.Loop.StartRound();

        Debug.Log($"After StartRound. Round: {GameBootstrap.Context.Rounds.CurrentRound}, State: {GameBootstrap.Context.StateMachine.CurrentState}");

        Refresh();
    }

    private void OnGameStateChanged(GameState state)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (button == null || GameBootstrap.Context == null)
            return;

        button.interactable = GameBootstrap.Context.StateMachine.IsInState(GameState.ShopBuild);
    }
}