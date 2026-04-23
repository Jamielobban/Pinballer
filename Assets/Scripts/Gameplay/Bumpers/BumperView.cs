using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BumperView : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string sourceId = "bumper";

    [Header("Tuning")]
    [SerializeField] private int baseValue = 2;
    [SerializeField] private float bounceForce = 10f;

    [Header("Feedback")]
    [SerializeField] private Transform visual;
    [SerializeField] private float hitScaleMultiplier = 1.15f;
    [SerializeField] private float scaleReturnSpeed = 10f;

    private Vector3 _baseScale;

    private void Awake()
    {
        if (visual == null)
            visual = transform;

        _baseScale = visual.localScale;
    }

    private void Update()
    {
        if (visual != null)
        {
            visual.localScale = Vector3.Lerp(visual.localScale, _baseScale, Time.deltaTime * scaleReturnSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BallView ballView = collision.collider.GetComponent<BallView>();
        if (ballView == null)
            return;

        BallRuntimeData ballData = ballView.RuntimeData;
        if (ballData == null)
            return;

        int perHitBonus = Mathf.RoundToInt(GameBootstrap.Context.Stats.GetMoneyPerHit());
        int bumperValue = Mathf.RoundToInt(GameBootstrap.Context.Stats.GetBumperHitValue());
        int finalValue = Mathf.Max(1, baseValue + perHitBonus + bumperValue - 1);

        ContactPoint2D contact = collision.GetContact(0);
        Vector2 pushDirection = (ballView.transform.position - transform.position).normalized;

        if (pushDirection.sqrMagnitude < 0.001f)
        {
            pushDirection = contact.normal;
        }

        float finalBounceForce = bounceForce * GameBootstrap.Context.Stats.GetBumperForce();
        ballView.Rigidbody.AddForce(pushDirection.normalized * finalBounceForce, ForceMode2D.Impulse);

        GameBootstrap.Context.Economy.AddMoney(finalValue);

        HitEventData hitData = new HitEventData
        {
            SourceId = sourceId,
            SourceType = HitSourceType.Bumper,
            BaseValue = baseValue,
            FinalValue = finalValue,
            Position = contact.point,
            Ball = ballData
        };

        GameBootstrap.Context.Signals.RaiseHitScored(hitData);

        if (visual != null)
        {
            visual.localScale = _baseScale * hitScaleMultiplier;
        }
    }
}