using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    [SerializeField] private Canvas _worldUI;
    [SerializeField] private Transform _mainCamTr;

    private void Start()
    {
        Camera mainCam = GameManager.Instance.MainCam;
        _worldUI.worldCamera = mainCam;
        _mainCamTr = mainCam.transform;
    }
    private void LateUpdate()
    {
        transform.LookAt(_mainCamTr.forward + transform.position);
    }
}
