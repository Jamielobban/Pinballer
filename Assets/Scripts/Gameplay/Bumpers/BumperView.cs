using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BumperView : MonoBehaviour, IPlaceableView
{
    [Header("Identity")]
    [SerializeField] private string sourceId = "bumper";

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

    public void Initialize(PlaceableRuntimeData runtime)
    {
        _runtime = runtime;
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

        RefreshDebug(); // 👈 add this
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameBootstrap.Context == null)
            return;

        BallView ballView = collision.collider.GetComponent<BallView>();
        if (ballView == null)
            return;

        BallRuntimeData ballData = ballView.RuntimeData;
        if (ballData == null)
            return;

        ContactPoint2D contact = collision.GetContact(0);

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

        if (_runtime != null)
        {
            bool shouldPayout = _runtime.RegisterHitAndCheckPayout();

            if (shouldPayout)
            {
                payout = CalculateFinalPayout(ballData);

                // Charged payout gives money.
                GameBootstrap.Context.Economy.AddMoney(payout);

                // Charged payout also gives score, affected by combo multiplier.
                GameBootstrap.Context.Score.AddScore(payout);

                _runtime.ResetCharge();
            }
        }
        else
        {
            payout = fallbackPayoutValue;

            GameBootstrap.Context.Economy.AddMoney(payout);
            GameBootstrap.Context.Score.AddScore(payout);
        }

        RaiseHitSignal(ballData, contact.point, payout);

        RefreshDebug();
        PlayHitFeedback();
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

        int ballMultiplier = Mathf.Max(1, ballData.ValueMultiplier);
        value *= ballMultiplier;
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

    private void RaiseHitSignal(BallRuntimeData ballData, Vector2 hitPoint, int finalValue)
    {
        HitEventData hitData = new HitEventData
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

        GameBootstrap.Context.Signals.RaiseHitScored(hitData);
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