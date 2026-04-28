using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BallView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;
    private BallRuntimeData _runtimeData;

    public Rigidbody2D Rigidbody => _rigidbody2D;
    public BallRuntimeData RuntimeData => _runtimeData;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Initialize(BallRuntimeData runtimeData)
    {
        _runtimeData = runtimeData;

        if (_runtimeData != null && _runtimeData.Definition != null)
            ApplyDefinition(_runtimeData.Definition);
    }

    private void ApplyDefinition(BallDefinition definition)
    {
        if (_rigidbody2D != null)
        {
            _rigidbody2D.mass = definition.Mass;
            _rigidbody2D.gravityScale = definition.GravityScale;
            _rigidbody2D.linearDamping = definition.LinearDrag;
            _rigidbody2D.angularDamping = definition.AngularDrag;
        }

        transform.localScale = Vector3.one * definition.SizeMultiplier;

        if (spriteRenderer != null)
        {
            if (definition.Sprite != null)
                spriteRenderer.sprite = definition.Sprite;

            spriteRenderer.color = definition.Tint;
        }

        if (_collider2D != null)
        {
            if (definition.PhysicsMaterial != null)
            {
                _collider2D.sharedMaterial = definition.PhysicsMaterial;
            }
            else
            {
                PhysicsMaterial2D material = new PhysicsMaterial2D($"{definition.DisplayName}_PhysicsMaterial");
                material.friction = definition.Friction;
                material.bounciness = definition.Bounciness;
                _collider2D.sharedMaterial = material;
            }
        }
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
}