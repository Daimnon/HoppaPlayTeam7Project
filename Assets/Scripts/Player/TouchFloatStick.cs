using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchFloatStick : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTr, _knobTr;
    public RectTransform RectTr => _rectTr;
    public RectTransform KnobTr => _knobTr;
}
