using System;
using UnityEngine;

public class GameplayInputReader : MonoBehaviour
{
    public event Action LaunchPressed;
    public event Action LaunchReleased;
    public event Action LeftFlipperPressed;
    public event Action LeftFlipperReleased;
    public event Action RightFlipperPressed;
    public event Action RightFlipperReleased;

    private PinballInputActions _inputActions;

    private void Awake()
    {
        _inputActions = new PinballInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Gameplay.Enable();

        _inputActions.Gameplay.Launch.started += OnLaunchStarted;
        _inputActions.Gameplay.Launch.canceled += OnLaunchCanceled;

        _inputActions.Gameplay.LeftFlipper.started += OnLeftFlipperStarted;
        _inputActions.Gameplay.LeftFlipper.canceled += OnLeftFlipperCanceled;

        _inputActions.Gameplay.RightFlipper.started += OnRightFlipperStarted;
        _inputActions.Gameplay.RightFlipper.canceled += OnRightFlipperCanceled;
    }

    private void OnDisable()
    {
        _inputActions.Gameplay.Launch.started -= OnLaunchStarted;
        _inputActions.Gameplay.Launch.canceled -= OnLaunchCanceled;

        _inputActions.Gameplay.LeftFlipper.started -= OnLeftFlipperStarted;
        _inputActions.Gameplay.LeftFlipper.canceled -= OnLeftFlipperCanceled;

        _inputActions.Gameplay.RightFlipper.started -= OnRightFlipperStarted;
        _inputActions.Gameplay.RightFlipper.canceled -= OnRightFlipperCanceled;

        _inputActions.Gameplay.Disable();
    }

    public bool IsLaunchHeld()
    {
        return _inputActions.Gameplay.Launch.IsPressed();
    }

    private void OnLaunchStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        LaunchPressed?.Invoke();
    }

    private void OnLaunchCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        LaunchReleased?.Invoke();
    }

    private void OnLeftFlipperStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        LeftFlipperPressed?.Invoke();
    }

    private void OnLeftFlipperCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        LeftFlipperReleased?.Invoke();
    }

    private void OnRightFlipperStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        RightFlipperPressed?.Invoke();
    }

    private void OnRightFlipperCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        RightFlipperReleased?.Invoke();
    }
}