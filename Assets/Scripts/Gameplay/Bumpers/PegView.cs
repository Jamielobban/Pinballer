using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PegView : MonoBehaviour, IPlaceableView, IBoardHittable
{
    [Header("Identity")]
    [SerializeField] private string sourceId = "peg";

    [Header("Default Runtime")]
    [SerializeField] private PlaceablePartDefinition defaultDefinition;

    [Header("Fallback Tuning")]
    [SerializeField] private int fallbackScoreValue = 1;
    [SerializeField] private int fallbackMoneyValue = 0;

    [Header("Feedback")]
    [SerializeField] private Transform visual;
    [SerializeField] private float hitScaleMultiplier = 1.1f;
    [SerializeField] private float scaleReturnSpeed = 10f;

    [Header("Runtime Debug")]
    [SerializeField] private PlaceableRuntimeData _runtime;
    [SerializeField] private int debugPayoutValue;

    private Vector3 _baseScale;

    public PlaceableRuntimeData RuntimeData => _runtime;

    private void Awake()
    {
        if (visual == null)
            visual = transform;

        _baseScale = visual.localScale;
    }

    private void Start()
    {
        EnsureRuntimeExists();
        RefreshDebug();
    }

    public void Initialize(PlaceableRuntimeData runtimeData)
    {
        _runtime = runtimeData;

        if (_runtime != null)
            _runtime.Instance = gameObject;

        RefreshDebug();
    }

    private void Update()
    {
        if (visual != null)
        {
            visual.localScale = Vector3.Lerp(
                visual.localScale,
                _baseScale,
                Time.deltaTime * scaleReturnSpeed
            );
        }

        RefreshDebug();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BallView ballView = collision.collider.GetComponent<BallView>();
        if (ballView == null)
            return;

        ContactPoint2D contact = collision.GetContact(0);

        HandleBallHit(ballView, contact);
    }

    public void HandleBallHit(BallView ballView, ContactPoint2D contact)
    {
        if (GameBootstrap.Context == null)
            return;

        EnsureRuntimeExists();

        if (ballView == null)
            return;

        BallRuntimeData ballData = ballView.RuntimeData;
        if (ballData == null)
            return;

        int payout = CalculateFinalPayout(ballData);

        if (payout > 0)
        {
            GameBootstrap.Context.Score.AddScore(payout);

            if (fallbackMoneyValue > 0)
                GameBootstrap.Context.Economy.AddMoney(fallbackMoneyValue);
        }

        HitEventData hitData = new HitEventData
        {
            SourceId = sourceId,
            SourceType = HitSourceType.Peg,
            BaseValue = GetBaseValue(),
            FinalValue = payout,
            Position = contact.point,
            Ball = ballData
        };

        GameBootstrap.Context.Signals.RaiseHitScored(hitData);
        BallAbilityRunner.OnHit(ballData, hitData);

        RefreshDebug();
        PlayHitFeedback();
    }

    private void EnsureRuntimeExists()
    {
        if (_runtime != null)
            return;

        if (defaultDefinition == null)
            return;

        _runtime = new PlaceableRuntimeData
        {
            Definition = defaultDefinition,
            Instance = gameObject
        };

        if (defaultDefinition.DefaultModifiers != null)
            _runtime.Modifiers.AddRange(defaultDefinition.DefaultModifiers);
    }

    private int CalculateFinalPayout(BallRuntimeData ballData)
    {
        int value = GetBaseValue();

        float ballMultiplier = ballData != null
            ? Mathf.Max(1f, ballData.ValueMultiplier)
            : 1f;

        value = Mathf.RoundToInt(value * ballMultiplier);

        return Mathf.Max(0, value);
    }

    private int GetBaseValue()
    {
        if (_runtime != null && _runtime.Definition != null)
            return _runtime.Definition.PayoutValue;

        return fallbackScoreValue;
    }

    private void RefreshDebug()
    {
        debugPayoutValue = GetBaseValue();
    }

    private void PlayHitFeedback()
    {
        if (visual != null)
            visual.localScale = _baseScale * hitScaleMultiplier;
    }
}