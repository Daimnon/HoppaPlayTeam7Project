using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchFloatStick : AspectRatioDetector
{
    [SerializeField] private RectTransform _rectTr, _knobTr;
    public RectTransform RectTr => _rectTr;
    public RectTransform KnobTr => _knobTr;

    [SerializeField] private CanvasScaler _scaler;
    [SerializeField] Vector2 _joystickBGSize = Vector2.zero;

    private void OnEnable()
    {
        Vector2 screenRes = new(Screen.width, Screen.height);
        _scaler.referenceResolution = screenRes;

        switch (_currentAspectRatio)
        {
            case DeviceType.OldPhone:
                break;
            case DeviceType.NewPhone:
                break;
            case DeviceType.Tablet:
                break;
            default:
                break;
        }
    }
    private void Start()
    {
        Vector2 screenRes = new(Screen.width, Screen.height);
        _scaler.referenceResolution = screenRes;
    }
}
