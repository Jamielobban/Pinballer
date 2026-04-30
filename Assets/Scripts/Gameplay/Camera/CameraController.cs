using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public enum CameraMode
    {
        Free,
        FollowBall
    }

    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Mode")]
    [SerializeField] private CameraMode mode = CameraMode.Free;
    [SerializeField] private Key followToggleKey = Key.F;
    [SerializeField] private float followSmooth = 8f;

    [Header("Follow Zoom Behavior")]
    [SerializeField] private float noFollowZoomThreshold = 22f;
    [SerializeField] private float fullXYFollowZoomThreshold = 12f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 20f;
    [SerializeField] private float zoomSmooth = 8f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 25f;

    [Header("Pan")]
    [SerializeField] private float dragPanSpeed = 1f;
    [SerializeField] private float inertiaDamping = 5f;
    [SerializeField] private float boundsSoftness = 8f;

    [Header("Board Bounds")]
    [SerializeField] private Vector2 boardMin = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 boardMax = new Vector2(10f, 10f);

    private Vector2 _lastMousePosition;
    private bool _isDragging;
    private Vector3 _panVelocity;
    private float _targetZoom;

    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        if (cam != null)
            _targetZoom = cam.orthographicSize;
    }

    private void Update()
    {
        HandleFollowToggle();

        bool zoomed = HandleZoom();

        if (mode == CameraMode.FollowBall)
            FollowCurrentBall();
        else
            HandleRightMouseDragPan();

        if (zoomed)
            HardClampCameraToBoardBounds();
        else
            SoftClampCameraToBoardBounds();
    }

    private void HandleFollowToggle()
    {
        if (Keyboard.current == null)
            return;

        if (followToggleKey == Key.F && Keyboard.current.fKey.wasPressedThisFrame)
        {
            mode = mode == CameraMode.Free ? CameraMode.FollowBall : CameraMode.Free;
            _panVelocity = Vector3.zero;
        }
    }

    private bool HandleZoom()
    {
        if (Mouse.current == null || cam == null)
            return false;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            _targetZoom = Mathf.Clamp(
                _targetZoom - scroll * zoomSpeed * Time.deltaTime,
                minZoom,
                maxZoom
            );
        }

        float oldZoom = cam.orthographicSize;

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            _targetZoom,
            zoomSmooth * Time.deltaTime
        );

        return Mathf.Abs(cam.orthographicSize - oldZoom) > 0.0001f;
    }

    private void FollowCurrentBall()
    {
        if (GameBootstrap.Context == null || cam == null)
            return;

        BallRuntimeData ball = GameBootstrap.Context.BallLifecycle.CurrentFollowBall;

        if (ball == null || ball.BallObject == null)
            return;

        if (cam.orthographicSize >= noFollowZoomThreshold)
            return;

        Vector3 current = transform.position;
        Vector3 ballPos = ball.BallObject.transform.position;

        Vector3 target = current;

        if (cam.orthographicSize <= fullXYFollowZoomThreshold)
        {
            target.x = ballPos.x;
            target.y = ballPos.y;
        }
        else
        {
            target.y = ballPos.y;
        }

        target.z = current.z;

        transform.position = Vector3.Lerp(
            current,
            target,
            Time.smoothDeltaTime * followSmooth
        );
    }

    private void HandleRightMouseDragPan()
    {
        if (Mouse.current == null || cam == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            _isDragging = true;
            _lastMousePosition = mousePosition;
            _panVelocity = Vector3.zero;
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            _isDragging = false;
        }

        if (_isDragging && Mouse.current.rightButton.isPressed)
        {
            Vector2 mouseDelta = mousePosition - _lastMousePosition;
            _lastMousePosition = mousePosition;

            float height = cam.orthographicSize * 2f;
            float width = height * cam.aspect;

            Vector3 worldDelta = new Vector3(
                -mouseDelta.x / Screen.width * width,
                -mouseDelta.y / Screen.height * height,
                0f
            );

            _panVelocity = worldDelta * dragPanSpeed / Mathf.Max(Time.deltaTime, 0.0001f);
        }

        transform.position += _panVelocity * Time.deltaTime;
        _panVelocity = Vector3.Lerp(_panVelocity, Vector3.zero, inertiaDamping * Time.deltaTime);
    }

    private void HardClampCameraToBoardBounds()
    {
        Vector3 pos = GetClampedPosition(transform.position);
        pos.z = transform.position.z;
        transform.position = pos;

        _panVelocity = Vector3.zero;
    }

    private void SoftClampCameraToBoardBounds()
    {
        if (cam == null)
            return;

        Vector3 pos = transform.position;
        Vector3 clamped = GetClampedPosition(pos);

        bool outsideX = Mathf.Abs(pos.x - clamped.x) > 0.001f;
        bool outsideY = Mathf.Abs(pos.y - clamped.y) > 0.001f;

        if (outsideX || outsideY)
        {
            pos = Vector3.Lerp(pos, clamped, boundsSoftness * Time.deltaTime);

            if (outsideX)
                _panVelocity.x = Mathf.Lerp(_panVelocity.x, 0f, boundsSoftness * Time.deltaTime);

            if (outsideY)
                _panVelocity.y = Mathf.Lerp(_panVelocity.y, 0f, boundsSoftness * Time.deltaTime);
        }

        pos.z = transform.position.z;
        transform.position = pos;
    }

    private Vector3 GetClampedPosition(Vector3 position)
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float minX = boardMin.x + halfWidth;
        float maxX = boardMax.x - halfWidth;

        float minY = boardMin.y + halfHeight;
        float maxY = boardMax.y - halfHeight;

        Vector3 clamped = position;

        if (minX > maxX)
            clamped.x = (boardMin.x + boardMax.x) * 0.5f;
        else
            clamped.x = Mathf.Clamp(position.x, minX, maxX);

        if (minY > maxY)
            clamped.y = (boardMin.y + boardMax.y) * 0.5f;
        else
            clamped.y = Mathf.Clamp(position.y, minY, maxY);

        return clamped;
    }
}