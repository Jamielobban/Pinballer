using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BumperView : MonoBehaviour, IPlaceableView, IBoardHittable
{
    [Header("Identity")]
    [SerializeField] private string sourceId = "bumper";

    [Header("Default Runtime")]
    [SerializeField] private PlaceablePartDefinition defaultDefinition;

    [Header("Fallback Tuning")]
    [SerializeField] private int fallbackPayoutValue = 2;
    [SerializeField] private int fallbackHitsRequired = 3;
    [SerializeField] private float fallbackBounceForce = 10f;

    [Header("Feedback")]
    [SerializeField] private Transform visual;
    [SerializeField] private float hitScaleMultiplier = 1.15f;
    [SerializeField] private float scaleReturnSpeed = 10f;

    [Header("Runtime Debug")]
    [SerializeField] private PlaceableRuntimeData _runtime;
    [SerializeField] private int debugCurrentHits;
    [SerializeField] private int debugHitsRequired;
    [SerializeField] private int debugPayoutValue;
    [SerializeField] private float debugChargePercent;

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

    public void Initialize(PlaceableRuntimeData runtime)
    {
        _runtime = runtime;

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

        float finalForce = CalculateFinalForce();

        ApplyBounce(ballView, contact, finalForce);

        PlaceableHitResolver.ResolveBasicHit(
            _runtime,
            ballData,
            contact.point,
            1,
            true
        );

        int payout = 0;

        HitEventData hitData = CreateHitData(
            ballData,
            contact.point,
            payout
        );

        if (_runtime != null)
        {
            bool shouldPayout = _runtime.RegisterHitAndCheckPayout();

            if (shouldPayout)
            {
                payout = CalculateFinalPayout(ballData);
                hitData.FinalValue = payout;

                GameBootstrap.Context.Economy.AddMoney(payout);
                GameBootstrap.Context.Score.AddScore(payout);

                _runtime.TriggerModifierPayoutEffects(ballData, hitData);

                _runtime.ResetCharge();
            }
        }
        else
        {
            payout = fallbackPayoutValue;
            hitData.FinalValue = payout;

            GameBootstrap.Context.Economy.AddMoney(payout);
            GameBootstrap.Context.Score.AddScore(payout);
        }

        GameBootstrap.Context.Signals.RaiseHitScored(hitData);
        BallAbilityRunner.OnHit(ballData, hitData);

        if (_runtime != null)
            _runtime.TriggerModifierHitEffects(ballData, hitData);

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
        int value = _runtime != null
            ? _runtime.GetFinalPayoutValue()
            : fallbackPayoutValue;

        int globalMoneyBonus = Mathf.RoundToInt(GameBootstrap.Context.Stats.GetMoneyPerHit());
        int globalBumperBonus = Mathf.RoundToInt(GameBootstrap.Context.Stats.GetBumperHitValue());

        value += globalMoneyBonus;
        value += globalBumperBonus;

        float ballMultiplier = Mathf.Max(1f, ballData.ValueMultiplier);
        value = Mathf.RoundToInt(value * ballMultiplier);

        return Mathf.Max(1, value);
    }

    private float CalculateFinalForce()
    {
        float force = _runtime != null
            ? _runtime.GetFinalForce()
            : fallbackBounceForce;

        force *= GameBootstrap.Context.Stats.GetBumperForce();

        return Mathf.Max(0f, force);
    }

    private void ApplyBounce(BallView ballView, ContactPoint2D contact, float force)
    {
        if (ballView == null || ballView.Rigidbody == null)
            return;

        Vector2 pushDirection = (ballView.transform.position - transform.position).normalized;

        if (pushDirection.sqrMagnitude < 0.001f)
            pushDirection = contact.normal;

        ballView.Rigidbody.AddForce(
            pushDirection.normalized * force,
            ForceMode2D.Impulse
        );
    }

    private HitEventData CreateHitData(
        BallRuntimeData ballData,
        Vector2 hitPoint,
        int finalValue)
    {
        return new HitEventData
        {
            SourceId = sourceId,
            SourceType = HitSourceType.Bumper,
            BaseValue = _runtime != null && _runtime.Definition != null
                ? _runtime.Definition.PayoutValue
                : fallbackPayoutValue,
            FinalValue = finalValue,
            Position = hitPoint,
            Ball = ballData
        };
    }

    private void RefreshDebug()
    {
        if (_runtime == null)
        {
            debugCurrentHits = 0;
            debugHitsRequired = fallbackHitsRequired;
            debugPayoutValue = fallbackPayoutValue;
            debugChargePercent = 0f;
            return;
        }

        debugCurrentHits = _runtime.CurrentHits;
        debugHitsRequired = _runtime.GetFinalHitsRequired();
        debugPayoutValue = _runtime.GetFinalPayoutValue();
        debugChargePercent = _runtime.GetChargePercent();
    }

    private void PlayHitFeedback()
    {
        if (visual != null)
            visual.localScale = _baseScale * hitScaleMultiplier;
    }
}