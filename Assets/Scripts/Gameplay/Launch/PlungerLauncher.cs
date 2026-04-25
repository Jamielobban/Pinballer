using UnityEngine;

public class PlungerLauncher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallFactory ballFactory;
    [SerializeField] private Transform loadedBallAnchor;
    [SerializeField] private GameplayInputReader inputReader;

    [Header("Launch Tuning")]
    [SerializeField] private float minLaunchForce = 5f;
    [SerializeField] private float maxLaunchForce = 18f;
    [SerializeField] private float chargeSpeed = 14f;
    [SerializeField] private Vector2 launchDirection = new Vector2(-1f, 1f);

    private BallView _loadedBallView;
    private BallRuntimeData _loadedBallData;
    private float _currentCharge;
    private bool _isCharging;
    private bool _subscribedToState;

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.LaunchPressed += OnLaunchPressed;
            inputReader.LaunchReleased += OnLaunchReleased;
        }
    }

    private void Start()
    {
        if (GameBootstrap.Context == null)
        {
            Debug.LogError("PlungerLauncher: GameBootstrap.Context is null.");
            return;
        }

        if (ballFactory == null)
        {
            Debug.LogError("PlungerLauncher: Missing BallFactory reference.");
            return;
        }

        if (loadedBallAnchor == null)
        {
            Debug.LogError("PlungerLauncher: Missing LoadedBallAnchor reference.");
            return;
        }

        if (inputReader == null)
        {
            Debug.LogError("PlungerLauncher: Missing GameplayInputReader reference.");
            return;
        }

        SubscribeToGameState();

        if (GameBootstrap.Context.StateMachine.IsInState(GameState.WaitingForBall))
        {
            TryLoadNextBallIfNeeded();
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.LaunchPressed -= OnLaunchPressed;
            inputReader.LaunchReleased -= OnLaunchReleased;
        }

        UnsubscribeFromGameState();
    }

    private void Update()
    {
        KeepLoadedBallSnapped();
        UpdateCharge();
    }

    private void SubscribeToGameState()
    {
        if (_subscribedToState)
            return;

        if (GameBootstrap.Context == null)
            return;

        GameBootstrap.Context.Signals.GameStateChanged += OnGameStateChanged;
        _subscribedToState = true;
    }

    private void UnsubscribeFromGameState()
    {
        if (!_subscribedToState)
            return;

        if (GameBootstrap.Context != null)
        {
            GameBootstrap.Context.Signals.GameStateChanged -= OnGameStateChanged;
        }

        _subscribedToState = false;
    }

    private void OnGameStateChanged(GameState state)
    {
        Debug.Log("Plunger heard state: " + state);

        if (state == GameState.WaitingForBall)
        {
            TryLoadNextBallIfNeeded();
        }
    }

    private void UpdateCharge()
    {
        if (!_isCharging)
            return;

        _currentCharge += chargeSpeed * Time.deltaTime;
        _currentCharge = Mathf.Clamp(_currentCharge, minLaunchForce, maxLaunchForce);
    }

    private void OnLaunchPressed()
    {
        BeginCharge();
    }

    private void OnLaunchReleased()
    {
        ReleaseLaunch();
    }

    private void BeginCharge()
    {
        if (_loadedBallView == null)
            return;

        if (_isCharging)
            return;

        _isCharging = true;
        _currentCharge = minLaunchForce;

        GameBootstrap.Context.Loop.OnLaunchStarted();
    }

    private void ReleaseLaunch()
    {
        if (_loadedBallView == null)
            return;

        if (!_isCharging)
            return;

        _isCharging = false;

        float launchPowerMultiplier = GameBootstrap.Context.Stats.GetLaunchPower();
        float finalForce = _currentCharge * launchPowerMultiplier;

        BallView launchingBallView = _loadedBallView;
        BallRuntimeData launchingBallData = _loadedBallData;

        _loadedBallView = null;
        _loadedBallData = null;

        PrepareBallForLaunch(launchingBallView);

        GameBootstrap.Context.Loop.OnBallLaunched();

        Vector2 force = launchDirection.normalized * finalForce;
        launchingBallView.Launch(force);

        launchingBallData.IsLoaded = false;
        launchingBallData.IsInPlay = true;
    }

    private void PrepareBallForLaunch(BallView ballView)
    {
        if (ballView == null)
            return;

        Rigidbody2D rb = ballView.Rigidbody;
        if (rb == null)
            return;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void TryLoadBall()
    {
        if (_loadedBallView != null)
            return;

        if (GameBootstrap.Context == null || GameBootstrap.Context.Loop == null)
            return;

        if (!GameBootstrap.Context.Loop.TryConsumeReserveForNextBall())
        {
            Debug.Log("Plunger tried to load ball, but reserve/state did not allow it.");
            return;
        }

        BallRuntimeData runtimeData = ballFactory.CreateBall(loadedBallAnchor.position);
        BallView ballView = runtimeData.BallObject.GetComponent<BallView>();

        runtimeData.IsLoaded = true;
        runtimeData.IsInPlay = false;

        ballView.SetKinematicLoadedState(true);

        _loadedBallView = ballView;
        _loadedBallData = runtimeData;

        GameBootstrap.Context.BallLifecycle.RegisterSpawn(runtimeData);
        GameBootstrap.Context.BallLifecycle.SetLoadedBall(runtimeData);
        GameBootstrap.Context.Loop.OnBallLoaded();

        Debug.Log("Loaded new ball.");
    }

    private void KeepLoadedBallSnapped()
    {
        if (_loadedBallView == null || loadedBallAnchor == null)
            return;

        _loadedBallView.transform.position = loadedBallAnchor.position;
    }

    public void TryLoadNextBallIfNeeded()
    {
        if (_loadedBallView == null)
        {
            TryLoadBall();
        }
    }
}