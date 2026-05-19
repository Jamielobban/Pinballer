using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class UISlideTrayView : MonoBehaviour
{
    public enum ScreenEdge
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public enum KeyOpenMode
    {
        Toggle,
        Hold
    }

    [Header("References")]
    [SerializeField] private RectTransform tray;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Behavior")]
    [SerializeField] private bool showOnMouseEdge = true;
    [SerializeField] private float edgeSize = 80f;
    [SerializeField] private ScreenEdge edge = ScreenEdge.Right;

    [Header("Keyboard")]
    [SerializeField] private Key toggleKey = Key.None;
    [SerializeField] private KeyOpenMode keyOpenMode = KeyOpenMode.Toggle;

    [Header("Close Behavior")]
    [SerializeField] private float closeDistance = 300f;
    [SerializeField] private float closeDelay = 0.25f;

    [Header("Tween")]
    [SerializeField] private float duration = 0.18f;
    [SerializeField] private Vector2 hiddenOffset = new Vector2(300f, 0f);

    private Vector2 _shownPosition;
    private Vector2 _hiddenPosition;

    private bool _isVisible;
    private bool _isAllowed;
    private float _closeTimer;

    private void Awake()
    {
        if (tray == null)
            tray = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        _shownPosition = tray.anchoredPosition;
        _hiddenPosition = _shownPosition + hiddenOffset;

        HideInstant();
    }

    private void Start()
    {
        if (GameBootstrap.Context != null)
            GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;

        RefreshAllowedState();
    }

    private void OnDestroy()
    {
        if (GameBootstrap.Context != null)
            GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
    }

    private void Update()
    {
        if (!_isAllowed)
            return;

        HandleKeyboardInput();

        if (Mouse.current == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (showOnMouseEdge && IsNearEdge(mousePos))
            Show();

        HandleAutoClose(mousePos);
    }

    private void HandleKeyboardInput()
    {
        if (toggleKey == Key.None || Keyboard.current == null)
            return;

        if (keyOpenMode == KeyOpenMode.Toggle)
        {
            if (WasKeyPressed(toggleKey))
                Toggle();
        }
        else
        {
            if (IsKeyHeld(toggleKey))
                Show();
            else if (!IsMouseKeepingTrayOpen())
                Hide();
        }
    }

    private bool IsMouseKeepingTrayOpen()
    {
        if (!showOnMouseEdge || Mouse.current == null)
            return false;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        return IsWithinCloseDistance(mousePos);
    }

    private void OnGameStateChanged(GameState state)
    {
        RefreshAllowedState();
    }

    private void RefreshAllowedState()
    {
        if (GameBootstrap.Context == null)
        {
            _isAllowed = false;
            Hide();
            return;
        }

        _isAllowed =
            GameBootstrap.Context.StateMachine.IsInState(GameState.WaitingForBall) 
            ||
            GameBootstrap.Context.StateMachine.IsInState(GameState.BallLoaded)
            ||
            GameBootstrap.Context.StateMachine.IsInState(GameState.BallLaunching)
            ||
            GameBootstrap.Context.StateMachine.IsInState(GameState.BallInPlay)
            ||
            GameBootstrap.Context.StateMachine.IsInState(GameState.ResolvingDrain)
            ||
            GameBootstrap.Context.StateMachine.IsInState(GameState.Paused)
            ||
            GameBootstrap.Context.StateMachine.IsInState(GameState.ShopBuild)
            ||
            GameBootstrap.Context.StateMachine.IsInState(GameState.LotteryDraw);;

        if (!_isAllowed)
            Hide();
    }

    private bool WasKeyPressed(Key key)
    {
        switch (key)
        {
            case Key.Tab: return Keyboard.current.tabKey.wasPressedThisFrame;
            case Key.Q: return Keyboard.current.qKey.wasPressedThisFrame;
            case Key.E: return Keyboard.current.eKey.wasPressedThisFrame;
            case Key.I: return Keyboard.current.iKey.wasPressedThisFrame;
            case Key.B: return Keyboard.current.bKey.wasPressedThisFrame;
            case Key.Space: return Keyboard.current.spaceKey.wasPressedThisFrame;
            default: return false;
        }
    }

    private bool IsKeyHeld(Key key)
    {
        switch (key)
        {
            case Key.Tab: return Keyboard.current.tabKey.isPressed;
            case Key.Q: return Keyboard.current.qKey.isPressed;
            case Key.E: return Keyboard.current.eKey.isPressed;
            case Key.I: return Keyboard.current.iKey.isPressed;
            case Key.B: return Keyboard.current.bKey.isPressed;
            case Key.Space: return Keyboard.current.spaceKey.isPressed;
            default: return false;
        }
    }

    private bool IsNearEdge(Vector2 mousePos)
    {
        switch (edge)
        {
            case ScreenEdge.Right: return mousePos.x >= Screen.width - edgeSize;
            case ScreenEdge.Left: return mousePos.x <= edgeSize;
            case ScreenEdge.Top: return mousePos.y >= Screen.height - edgeSize;
            case ScreenEdge.Bottom: return mousePos.y <= edgeSize;
            default: return false;
        }
    }

   private void HandleAutoClose(Vector2 mousePos)
    {
        if (!_isVisible)
            return;

        // Toggle mode should stay open until the key/button toggles it closed.
        if (keyOpenMode == KeyOpenMode.Toggle)
            return;

        if (keyOpenMode == KeyOpenMode.Hold && IsKeyHeld(toggleKey))
            return;

        bool hasSelectedInventoryItem =
            GameBootstrap.Context != null &&
            GameBootstrap.Context.Inventory != null &&
            GameBootstrap.Context.Inventory.SelectedItem != null;

        bool farFromEdge = !IsWithinCloseDistance(mousePos);

        if (!hasSelectedInventoryItem && farFromEdge)
        {
            _closeTimer += Time.deltaTime;

            if (_closeTimer >= closeDelay)
                Hide();
        }
        else
        {
            _closeTimer = 0f;
        }
    }

    private bool IsWithinCloseDistance(Vector2 mousePos)
    {
        switch (edge)
        {
            case ScreenEdge.Right: return mousePos.x >= Screen.width - closeDistance;
            case ScreenEdge.Left: return mousePos.x <= closeDistance;
            case ScreenEdge.Top: return mousePos.y >= Screen.height - closeDistance;
            case ScreenEdge.Bottom: return mousePos.y <= closeDistance;
            default: return false;
        }
    }

    public void Toggle()
    {
        if (_isVisible)
            Hide();
        else
            Show();
    }

    public void Show()
    {
        if (!_isAllowed || _isVisible)
            return;

        _isVisible = true;
        _closeTimer = 0f;

        gameObject.SetActive(true);

        tray.DOKill();
        canvasGroup.DOKill();

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        tray.DOAnchorPos(_shownPosition, duration).SetEase(Ease.OutCubic);
        canvasGroup.DOFade(1f, duration);
    }

    public void Hide()
    {
        if (!_isVisible)
            return;

        _isVisible = false;
        _closeTimer = 0f;

        tray.DOKill();
        canvasGroup.DOKill();

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        tray.DOAnchorPos(_hiddenPosition, duration).SetEase(Ease.InCubic);
        canvasGroup.DOFade(0f, duration);
    }

    private void HideInstant()
    {
        _isVisible = false;
        _closeTimer = 0f;

        if (tray != null)
            tray.anchoredPosition = _hiddenPosition;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}