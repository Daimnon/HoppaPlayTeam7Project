using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerController : Character
{
    [SerializeField] Animator _playerAnimator;
    [SerializeField] private TouchFloatStick _stick;
    [SerializeField] private Vector2 _stickSize = new(300.0f, 300.0f);
    [SerializeField] private Vector2 _screenOffsetMargin = new(100.0f, 50.0f);
    [SerializeField] private float _idleGestureTime = 7.5f;
    [SerializeField] private NavMeshAgent _agent;

    private float _idleTime = 0.0f;
    private bool _isGesturing = false;

    private Finger _moveFinger;
    private Vector3 _fingerMoveAmount;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += OnFingerDown;
        ETouch.Touch.onFingerUp += OnFingerUp;
        ETouch.Touch.onFingerMove += OnFingerMove;
    }
    private void Update()
    {
        Vector3 scaledMovement = _agent.speed * Time.deltaTime * new Vector3(_fingerMoveAmount.x, 0, _fingerMoveAmount.y);

        _agent.transform.LookAt(_agent.transform.position + scaledMovement, Vector3.up);
        _agent.Move(scaledMovement);

        _playerAnimator.SetFloat("Move Speed", scaledMovement.normalized.magnitude);

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
        EnhancedTouchSupport.Disable();
    }

    private void OnFingerMove(Finger finger)
    {
        if (finger == _moveFinger)
        {
            Vector2 knobPos;
            float maxMoveLenght = _stickSize.x / 2;
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
        if (finger == _moveFinger)
        {
            _moveFinger = null;
            _stick.KnobTr.anchoredPosition = Vector2.zero;
            _stick.gameObject.SetActive(false);
            _fingerMoveAmount = Vector3.zero;
        }
    }
    private void OnFingerDown(Finger finger)
    {
        Vector2 touchPosition = finger.screenPosition;

        // calculate the bounds of the margin
        float leftMargin = _screenOffsetMargin.x;
        float rightMargin = Screen.width - _screenOffsetMargin.x;
        float topMargin = Screen.height - _screenOffsetMargin.y;
        float bottomMargin = _screenOffsetMargin.y;

        if (_moveFinger == null && !(touchPosition.x < leftMargin || touchPosition.x > rightMargin || touchPosition.y < bottomMargin || touchPosition.y > topMargin))
        {
            _moveFinger = finger;
            _fingerMoveAmount = Vector3.zero;
            _stick.gameObject.SetActive(true);
            _stick.RectTr.sizeDelta = _stickSize;
            _stick.RectTr.anchoredPosition = ClampStickDownPos(finger.screenPosition);
        }
    }
    private Vector2 ClampStickDownPos(Vector2 stickPos)
    {
        float halfWidth = _stickSize.x / 2.0f;
        float halfHeight = _stickSize.y / 2.0f;

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
}
