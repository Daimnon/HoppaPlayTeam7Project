using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchFloatStick : AspectRatioDetector
{
    [SerializeField] private CanvasScaler _scaler;

    [SerializeField] private RectTransform _rectTr, _knobOutline, _knobTr;
    public RectTransform RectTr => _rectTr;
    public RectTransform KnobOutline => _knobOutline;
    public RectTransform KnobTr => _knobTr;

    #region Old Phone Properties
    [Header("[9:16] Old Phone (default)")]
    [SerializeField] private Vector2 _joystickBgSizeOld = Vector2.zero;
    public Vector2 JoystickBgSizeOld => _joystickBgSizeOld;

    [SerializeField] private Vector2 _joystickKnobSizeOld = Vector2.zero;
    public Vector2 JoystickKnobSizeOld => _joystickKnobSizeOld;
    #endregion

    #region New Phone Properties
    [Header("[9:19.5] New Phone")]
    [SerializeField] private Vector2 _joystickBgSizeNew = new(300.0f, 300.0f);
    public Vector2 JoystickBgSizeNew => _joystickBgSizeNew;

    [SerializeField] private Vector2 _joystickKnobSizeNew = Vector2.zero;
    public Vector2 JoystickKnobSizeNew => _joystickKnobSizeNew;
    #endregion

    #region Tab Phone Properties
    [Header("[3:4] Tablet")]
    [SerializeField] private Vector2 _joystickBgSizeTab = Vector2.zero;
    public Vector2 JoystickBgSizeTab => _joystickBgSizeTab;

    [SerializeField] private Vector2 _joystickKnobSizeTab = Vector2.zero;
    public Vector2 JoystickKnobSizeTab => _joystickKnobSizeTab;
    #endregion

    protected override void OnEnable()
    {
        base.OnEnable();

        ApplyAspectRactioToJoystick();
    }
    protected override void Start()
    {
        base.Start();

        ApplyAspectRactioToJoystick();
    }

    private void ApplyAspectRactioToJoystick()
    {
        Vector2 screenRes = new(Screen.width, Screen.height);
        _scaler.referenceResolution = screenRes;

        SetJoystickSize();
    }

    public void SetJoystickSize()
    {
        switch (_currentAspectRatio)
        {
            case DeviceType.OldPhone:
                _rectTr.sizeDelta = _joystickBgSizeOld;
                _knobOutline.sizeDelta = _joystickKnobSizeOld;
                break;
            case DeviceType.NewPhone:
                _rectTr.sizeDelta = _joystickBgSizeNew;
                _knobOutline.sizeDelta = _joystickKnobSizeNew;
                break;
            case DeviceType.Tablet:
                _rectTr.sizeDelta = _joystickBgSizeTab;
                _knobOutline.sizeDelta = _joystickKnobSizeTab;
                break;
            default:
                _rectTr.sizeDelta = _joystickBgSizeOld;
                _knobOutline.sizeDelta = _joystickKnobSizeOld;
                break;
        }
    }
}
