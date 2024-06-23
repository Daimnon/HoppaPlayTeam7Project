using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class Player_Controller : Character
{
    [Header("MVC Components")]
    [SerializeField] private Player_Data _data;

    [Header("Components")]
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private TouchFloatStick _stick;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private NavMeshSurface _navMeshSurface;

    [Header("Screen")]
    [SerializeField] private Vector2 _screenEdgeOffsetMargin = new(100.0f, 50.0f);

    [Header("Animation")]
    [SerializeField] private float _idleGestureTime = 7.5f;

    private float _idleTime = 0.0f;
    private bool _isGesturing = false;

    private Finger _moveFinger;
    private Vector3 _fingerMoveAmount;

    #region Monobehaviour Callbacks
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += OnFingerDown;
        ETouch.Touch.onFingerUp += OnFingerUp;
        ETouch.Touch.onFingerMove += OnFingerMove;
        EventManager.OnGrowth += OnGrowth;
        EventManager.OnEvolve += OnEvolve;
        _navMeshSurface.BuildNavMesh();
    }
    private void Update()
    {
        /* movement */
        Vector3 scaledMovement = _agent.speed * Time.deltaTime * new Vector3(_fingerMoveAmount.x, 0, _fingerMoveAmount.y);

        _agent.transform.LookAt(_agent.transform.position + scaledMovement, Vector3.up);
        _agent.Move(scaledMovement);

        /* animation */
        _playerAnimator.SetFloat("Move Speed", scaledMovement.normalized.magnitude);

        /* idle gesture */
        if (scaledMovement == Vector3.zero)
        {
            _idleTime += Time.deltaTime;

            if (_idleTime >= _idleGestureTime && !_isGesturing)
            {
                _isGesturing = true;
                _playerAnimator.SetBool("Is Gesturing", _isGesturing);
            }
        }
        else if (_idleTime != 0)
        {
            _idleTime = 0;

            _isGesturing = false;
            _playerAnimator.SetBool("Is Gesturing", _isGesturing);
        }
    }
    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= OnFingerDown;
        ETouch.Touch.onFingerUp -= OnFingerUp;
        ETouch.Touch.onFingerMove -= OnFingerMove;
        EventManager.OnGrowth -= OnGrowth;
        EnhancedTouchSupport.Disable();
    }
    private void OnTriggerEnter(Collider other) 
    {
        if (other.TryGetComponent(out Consumable consumable) && consumable.transform.localScale.x <= transform.localScale.x && consumable.transform.localScale.z <= transform.localScale.z)
        {
            _data.GainExp(consumable.expValue);
            Destroy(other.gameObject);
            UpdateNavMesh();
        }
    }
    #endregion

    #region Fingers
    private void OnFingerDown(Finger finger)
    {
        /* get touch input position */
        Vector2 touchPosition = finger.screenPosition;

        /* calculate the bounds of the margin */
        float leftMargin = _screenEdgeOffsetMargin.x;
        float rightMargin = Screen.width - _screenEdgeOffsetMargin.x;
        float topMargin = Screen.height - _screenEdgeOffsetMargin.y;
        float bottomMargin = _screenEdgeOffsetMargin.y;

        /* making sure we are indeed starting the touch */
        if (_moveFinger == null && !(touchPosition.x < leftMargin || touchPosition.x > rightMargin || touchPosition.y < bottomMargin || touchPosition.y > topMargin))
        {
            _moveFinger = finger;
            _fingerMoveAmount = Vector3.zero;
            _stick.gameObject.SetActive(true);
            _stick.SetJoystickSize();

            // Adjust position based on aspect ratio
            _stick.RectTr.anchoredPosition = ClampStickDownPos(AdjustForAspectRatio(touchPosition));
        }
    }
    private void OnFingerMove(Finger finger)
    {
        /* joystick movement */
        if (finger == _moveFinger)
        {
            Vector2 knobPos; // joystick knob
            float maxMoveLenght = _stick.RectTr.sizeDelta.x / 2;
            ETouch.Touch currentTouch = finger.currentTouch;

            if (Vector2.Distance(currentTouch.screenPosition, _stick.RectTr.anchoredPosition) > maxMoveLenght)
                knobPos = (currentTouch.screenPosition - _stick.RectTr.anchoredPosition).normalized * maxMoveLenght;
            else
                knobPos = currentTouch.screenPosition - _stick.RectTr.anchoredPosition;

            _stick.KnobTr.anchoredPosition = knobPos;
            _fingerMoveAmount = knobPos / maxMoveLenght;
        }
    }
    private void OnFingerUp(Finger finger)
    {
        /* reset joystick */
        if (finger == _moveFinger)
        {
            _moveFinger = null;
            _stick.KnobTr.anchoredPosition = Vector2.zero;
            _stick.gameObject.SetActive(false);
            _fingerMoveAmount = Vector3.zero;
        }
    }
    #endregion

    #region Touch Calculations
    private Vector2 AdjustForAspectRatio(Vector2 position)
    {
        float aspectRatio = (float)Screen.width / Screen.height;
        float distanceOffset = Mathf.Lerp(1.0f, 2.0f, (aspectRatio - 1.0f) / (2.0f - 1.0f)); // Adjust this as needed

        return new Vector2(position.x * distanceOffset, position.y * distanceOffset);
    }
    private Vector2 ClampStickDownPos(Vector2 stickPos)
    {
        /* help us keep joystick knob in the correct position in relation to joystick bg */
        float halfWidth = _stick.RectTr.sizeDelta.x / 2.0f;
        float halfHeight = _stick.RectTr.sizeDelta.y / 2.0f;

        float screenWidthMinusHalfWidth = Screen.width - halfWidth;
        float screenHeightMinusHalfHeight = Screen.height - halfHeight;

        if (stickPos.x < halfWidth)
            stickPos.x = halfWidth;
        else if (stickPos.x > screenWidthMinusHalfWidth)
            stickPos.x = screenWidthMinusHalfWidth;

        if (stickPos.y < halfHeight)
            stickPos.y = halfHeight;
        else if (stickPos.y > screenHeightMinusHalfHeight)
            stickPos.y = screenHeightMinusHalfHeight;

        return stickPos;
    }
    #endregion

    #region General Methods
    private void UpdateNavMesh()
    {
        if (_navMeshSurface)
            _navMeshSurface.BuildNavMesh();
    }
    #endregion

    #region Events
    private void OnGrowth()
    {
        transform.localScale += Vector3.one * _data.ScaleIncrement;
    }
    private void OnEvolve(EvoType newEvoType)
    {
        int newEvoTypeNum = (int)newEvoType;
        if (!_data.EvoModels[newEvoTypeNum] || newEvoTypeNum - 1 < 0) // models existance
            return;

        _data.EvoModels[newEvoTypeNum - 1].SetActive(false);
        _data.EvoModels[newEvoTypeNum].SetActive(true);
    }
    #endregion
}