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
            return;

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
        if (!CanStartRound())
            return;

        GameBootstrap.Context.Loop.StartRound();
        Refresh();
    }

    private void OnGameStateChanged(GameState state)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (button == null)
            return;

        button.interactable = CanStartRound();
    }

    private bool CanStartRound()
    {
        if (GameBootstrap.Context == null)
            return false;

        return GameBootstrap.Context.StateMachine.IsInState(GameState.ShopBuild)
            || GameBootstrap.Context.StateMachine.IsInState(GameState.BoardEdit);
    }
}