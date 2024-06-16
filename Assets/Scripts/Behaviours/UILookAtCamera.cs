using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private Canvas _worldUI;
    [SerializeField] private Transform _mainCamTr;

    private void Start()
    {
        _worldUI.worldCamera = _cam;
        _mainCamTr = _cam.transform;
    }
    private void LateUpdate()
    {
        transform.LookAt(_mainCamTr.forward + transform.position);
    }
}
