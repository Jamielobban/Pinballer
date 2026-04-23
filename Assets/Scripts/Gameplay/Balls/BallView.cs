using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallView : MonoBehaviour
{
    [SerializeField] private float minVelocityForInPlay = 0.5f;

    private Rigidbody2D _rigidbody2D;
    private BallRuntimeData _runtimeData;
    private bool _hasEnteredPlayfield;

    public Rigidbody2D Rigidbody => _rigidbody2D;
    public BallRuntimeData RuntimeData => _runtimeData;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Initialize(BallRuntimeData runtimeData)
    {
        _runtimeData = runtimeData;
        _hasEnteredPlayfield = false;
    }

    public void SetKinematicLoadedState(bool isLoaded)
    {
        if (_rigidbody2D == null)
            return;

        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0f;
        _rigidbody2D.bodyType = isLoaded ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
    }

    public void Launch(Vector2 force)
    {
        if (_rigidbody2D == null)
            return;

        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (_runtimeData == null || _rigidbody2D == null)
            return;

        if (!_hasEnteredPlayfield && _rigidbody2D.linearVelocity.magnitude > minVelocityForInPlay)
        {
            _hasEnteredPlayfield = true;
        }
    }
}