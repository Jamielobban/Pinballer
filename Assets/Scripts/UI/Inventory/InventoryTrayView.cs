using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTrayView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform tray;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Behavior")]
    [SerializeField] private bool showOnMouseEdge = true;
    [SerializeField] private float edgeWidth = 80f;
    [SerializeField] private Key toggleKey = Key.Tab;

    [Header("Tween")]
    [SerializeField] private float duration = 0.18f;
    [SerializeField] private Vector2 hiddenOffset = new Vector2(320f, 0f);

    [SerializeField] private float closeEdgeDistance = 420f;
    [SerializeField] private float closeDelay = 0.25f;

    private float _closeTimer;

    private Vector2 _shownPosition;
    private Vector2 _hiddenPosition;
    private bool _isVisible;
    private bool _isAllowed;

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

        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            Toggle();
        }

        if (showOnMouseEdge && Mouse.current != null)
        {
            float mouseX = Mouse.current.position.ReadValue().x;
            bool nearRightEdge = mouseX >= Screen.width - edgeWidth;

            if (nearRightEdge)
                Show();
        }
        
        if (_isVisible && Mouse.current != null)
        {
            bool hasSelectedItem = GameBootstrap.Context.Inventory.SelectedItem != null;
            float mouseX = Mouse.current.position.ReadValue().x;
            bool mouseFarFromRightSide = mouseX < Screen.width - closeEdgeDistance;

            if (!hasSelectedItem && mouseFarFromRightSide)
            {
                _closeTimer += Time.deltaTime;

                if (_closeTimer >= closeDelay)
                {
                    Hide();
                }
            }
            else
            {
                _closeTimer = 0f;
            }
        }
    }

    private void OnGameStateChanged(GameState state)
    {
        RefreshAllowedState();
    }

    private void RefreshAllowedState()
    {
        _isAllowed = GameBootstrap.Context.StateMachine.IsInState(GameState.BoardEdit);

        if (!_isAllowed)
            Hide();
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

        tray.anchoredPosition = _hiddenPosition;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}