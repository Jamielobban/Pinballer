using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BumperView : MonoBehaviour, IPlaceableView
{
    [Header("Identity")]
    [SerializeField] private string sourceId = "bumper";

    [Header("Fallback Tuning")]
    [SerializeField] private int fallbackBaseValue = 2;
    [SerializeField] private float fallbackBounceForce = 10f;

    [Header("Feedback")]
    [SerializeField] private Transform visual;
    [SerializeField] private float hitScaleMultiplier = 1.15f;
    [SerializeField] private float scaleReturnSpeed = 10f;

    private Vector3 _baseScale;
    [SerializeField] private PlaceableRuntimeData _runtime;

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

        int finalValue = CalculateFinalValue(ballData);
        float finalForce = CalculateFinalForce();

        ApplyBounce(ballView, contact, finalForce);
        ApplyModifierEffects(ballData, contact.point);

        GameBootstrap.Context.Economy.AddMoney(finalValue);

        RaiseHitSignal(ballData, contact.point, finalValue);

        PlayHitFeedback();
    }

    private int CalculateFinalValue(BallRuntimeData ballData)
    {
        int value = _runtime != null
            ? _runtime.GetFinalValue()
            : fallbackBaseValue;

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

    private void ApplyModifierEffects(BallRuntimeData ballData, Vector2 hitPoint)
    {
        if (_runtime == null)
            return;

        if (_runtime.HasModifier(ModifierType.Explode))
        {
            Debug.Log("Explode modifier triggered at " + hitPoint);
        }

        if (_runtime.HasModifier(ModifierType.Chain))
        {
            Debug.Log("Chain modifier triggered at " + hitPoint);
        }
    }

    private void RaiseHitSignal(BallRuntimeData ballData, Vector2 hitPoint, int finalValue)
    {
        HitEventData hitData = new HitEventData
        {
            SourceId = sourceId,
            SourceType = HitSourceType.Bumper,
            BaseValue = _runtime != null && _runtime.Definition != null
                ? _runtime.Definition.BaseValue
                : fallbackBaseValue,
            FinalValue = finalValue,
            Position = hitPoint,
            Ball = ballData
        };

        GameBootstrap.Context.Signals.RaiseHitScored(hitData);
    }

    private void PlayHitFeedback()
    {
        if (visual != null)
            visual.localScale = _baseScale * hitScaleMultiplier;
    }
}