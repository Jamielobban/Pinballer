using UnityEngine;

[RequireComponent(typeof(HingeJoint2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class FlipperView : MonoBehaviour
{
    public enum FlipperSide
    {
        Left,
        Right
    }

    [Header("Setup")]
    [SerializeField] private FlipperSide side;
    [SerializeField] private GameplayInputReader inputReader;

    [Header("Angles")]
    [SerializeField] private float restAngle = 0f;
    [SerializeField] private float activeAngle = 60f;

    [Header("Motor")]
    [SerializeField] private float motorSpeed = 1000f;
    [SerializeField] private float motorForce = 10000f;

    private HingeJoint2D _hinge;
    private JointMotor2D _motor;

    private void Awake()
    {
        _hinge = GetComponent<HingeJoint2D>();

        _motor = _hinge.motor;
        _motor.motorSpeed = 0f;
        _motor.maxMotorTorque = motorForce;

        _hinge.useMotor = true;
    }

    private void OnEnable()
    {
        if (inputReader == null)
            return;

        if (side == FlipperSide.Left)
        {
            inputReader.LeftFlipperPressed += OnPressed;
            inputReader.LeftFlipperReleased += OnReleased;
        }
        else
        {
            inputReader.RightFlipperPressed += OnPressed;
            inputReader.RightFlipperReleased += OnReleased;
        }
    }

    private void OnDisable()
    {
        if (inputReader == null)
            return;

        if (side == FlipperSide.Left)
        {
            inputReader.LeftFlipperPressed -= OnPressed;
            inputReader.LeftFlipperReleased -= OnReleased;
        }
        else
        {
            inputReader.RightFlipperPressed -= OnPressed;
            inputReader.RightFlipperReleased -= OnReleased;
        }
    }

    private void OnPressed()
    {
        SetMotorTowards(activeAngle);
    }

    private void OnReleased()
    {
        SetMotorTowards(restAngle);
    }

    private void SetMotorTowards(float targetAngle)
    {
        float currentAngle = _hinge.jointAngle;

        float direction = Mathf.Sign(targetAngle - currentAngle);
        _motor.motorSpeed = direction * motorSpeed;

        _hinge.motor = _motor;
    }
}