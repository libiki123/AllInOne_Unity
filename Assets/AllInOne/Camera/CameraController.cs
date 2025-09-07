using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _camTarget;
    [SerializeField] private CinemachineOrbitalFollow _orbitalFollow;

    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private AnimationCurve _moveSpeedZoomCurve = AnimationCurve.Linear(0, 0.5f, 1f, 1f);
    [SerializeField] private float _aceleration = 10f;
    [SerializeField] private float _deceleration = 10f;
    [SerializeField] private float _sprintMultiplier = 2f;

    [SerializeField] private float _edgeScrollMargin = 15f;

    [SerializeField] private float _zoomSpeed = 0.05f;
    [SerializeField] private float _zoomSmoothing = 10f;

    [SerializeField] float _orbitSensitivity = 0.5f;
    [SerializeField] float _orbitSmoothing = 100f;

    private Camera _mainCam;
    private Vector3 _velocity = Vector3.zero;
    private float _currentZoomSpeed;
    private Vector2 _edgeScrollInput;
    private float decelerationMultiplier = 1f;

    private float _zoomLevel
    {
        get
        {
            InputAxis zoomAxis = _orbitalFollow.RadialAxis;
            return Mathf.InverseLerp(zoomAxis.Range.x, zoomAxis.Range.y, zoomAxis.Value); // 0 = zoomed in, 1 = zoomed out
        }
    }

    #region Inout

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Vector2 _scrollInput;
    private bool _sprintInput;
    private bool _rightClickInput;

    void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        _lookInput = value.Get<Vector2>();
    }

    void OnScrollWheel(InputValue value)
    {
        _scrollInput = value.Get<Vector2>();
    }

    void OnRightClick(InputValue value)
    {
        _rightClickInput = value.isPressed;
    }

    void OnSprint(InputValue value)
    {
        _sprintInput = value.isPressed;
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        float deltaTime = Time.unscaledDeltaTime;  // Use unscaled time to ignore time scale changes (in case of slow motion effects / pauses)

        if (!Application.isEditor)
            UpdateEdgeScrolling();
        UpdateOrbit(deltaTime);
        UpdateMovement(deltaTime);
        UpdateZoom(deltaTime);
    }

    #endregion

    #region Control Methods

    private void UpdateEdgeScrolling()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        _edgeScrollInput = Vector2.zero;

        if (mousePosition.x <= _edgeScrollMargin)
        {
            _edgeScrollInput.x = -1;
        }
        else if (mousePosition.x >= screenSize.x - _edgeScrollMargin)
        {
            _edgeScrollInput.x = 1;
        }

        if (mousePosition.y <= _edgeScrollMargin)
        {
            _edgeScrollInput.y = -1;
        }
        else if (mousePosition.y >= screenSize.y - _edgeScrollMargin)
        {
            _edgeScrollInput.y = 1;
        }
    }

    private void UpdateMovement(float deltaTime)
    {
        Vector3 forward = _mainCam.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = _mainCam.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 inputVector = new Vector3(_moveInput.x + _edgeScrollInput.x, 0, _moveInput.y + _edgeScrollInput.y);
        inputVector.Normalize();

        float zoonMultiplier = _moveSpeedZoomCurve.Evaluate(_zoomLevel);
        Vector3 targetVelocity = inputVector * _moveSpeed * zoonMultiplier;
        float sprintFactor = 1f;

        if (_sprintInput)
        {
            targetVelocity *= _sprintMultiplier;
            sprintFactor = _sprintMultiplier;
        }

        if (inputVector.sqrMagnitude > 0.01f) // check if move input being held down
        {
            _velocity = Vector3.MoveTowards(_velocity, targetVelocity, deltaTime * _aceleration * sprintFactor);

            if (_sprintInput)
            {
                decelerationMultiplier = _sprintMultiplier;
            }
        }
        else
        {
            _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, deltaTime * _deceleration * decelerationMultiplier);
        }

        Vector3 motion = _velocity * deltaTime;
        _camTarget.position += forward * motion.z + right * motion.x;

        if (_velocity.sqrMagnitude < 0.01f)
        {
            decelerationMultiplier = 1f;
        }
    }

    private void UpdateOrbit(float deltaTime)
    {
        Vector2 orbitInput = _lookInput * _orbitSensitivity * (_rightClickInput ? 1f : 0f); // Only orbit when middle mouse button is held down
        InputAxis horizontalAxis = _orbitalFollow.HorizontalAxis;
        InputAxis verticalAxis = _orbitalFollow.VerticalAxis;

        // horizontalAxis.Value += orbitInput.x;
        // verticalAxis.Value -= orbitInput.y;

        horizontalAxis.Value = Mathf.Lerp(horizontalAxis.Value, horizontalAxis.Value + orbitInput.x, deltaTime * _orbitSmoothing);
        verticalAxis.Value = Mathf.Lerp(verticalAxis.Value, verticalAxis.Value - orbitInput.y, deltaTime * _orbitSmoothing);
        verticalAxis.Value = Mathf.Clamp(verticalAxis.Value, verticalAxis.Range.x, verticalAxis.Range.y); // Range is set in the cinemachine orbital follow component

        _orbitalFollow.HorizontalAxis = horizontalAxis;
        _orbitalFollow.VerticalAxis = verticalAxis;
    }

    private void UpdateZoom(float deltaTime)
    {
        InputAxis zoomAxis = _orbitalFollow.RadialAxis;
        float targetZoomSpeed = 0f;

        if (Mathf.Abs(_scrollInput.y) >= 0.01f)
        {
            targetZoomSpeed = _zoomSpeed * _scrollInput.y;
        }

        _currentZoomSpeed = Mathf.Lerp(_currentZoomSpeed, targetZoomSpeed, deltaTime * _zoomSmoothing); // Smooth zoom speed changes
        zoomAxis.Value -= _currentZoomSpeed;
        zoomAxis.Value = Mathf.Clamp(zoomAxis.Value, zoomAxis.Range.x, zoomAxis.Range.y); // Range is set in the cinemachine orbital follow component

        _orbitalFollow.RadialAxis = zoomAxis;
    }

    #endregion
}