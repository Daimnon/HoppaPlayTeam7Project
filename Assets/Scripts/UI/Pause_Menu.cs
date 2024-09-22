using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause_Menu : MonoBehaviour
{
    [SerializeField] private GameObject[] _windowsToTurnOff; // 0 = off, 1 = on.
    [SerializeField] private Sprite[] _switchSprites; // 0 = off, 1 = on.
    [SerializeField] private Image _soundSwitch;
    [SerializeField] private Image _hapticSwitch;

    bool[] _tempOriginalWindowsActive;

    private bool _isSoundOn = true;
    private bool _isHapticOn = true;

    private bool _isPaused = false;

    private void OnEnable()
    {
        InitializeOtherWindows();

        Time.timeScale = 0.0f;
        _isPaused = true;
        EventManager.InvokePause(true);

        HandlePauseOtherWindows();
    }
    private void OnDisable()
    {
        Time.timeScale = 1.0f;
        _isPaused = false;

        HandleUnPauseOtherWindows();
        EventManager.InvokePause(false);
    }

    private void InitializeOtherWindows()
    {
        _tempOriginalWindowsActive = new bool[_windowsToTurnOff.Length];
        for (int i = 0; i < _windowsToTurnOff.Length; i++)
        {
            _tempOriginalWindowsActive[i] = _windowsToTurnOff[i].activeInHierarchy;
        }
    }
    private void HandlePauseOtherWindows()
    {
        for (int i = 0; i < _windowsToTurnOff.Length; i++)
        {
            _windowsToTurnOff[i].SetActive(false);
        }
    }
    private void HandleUnPauseOtherWindows()
    {
        for (int i = 0; i < _windowsToTurnOff.Length; i++)
        {
            _windowsToTurnOff[i].SetActive(_tempOriginalWindowsActive[i]);
        }
    }

    public void TurnSoundSwitch()
    {
        switch (_isSoundOn)
        {
            case true:
                _soundSwitch.sprite = _switchSprites[0];
                _isSoundOn = false;
                break;
            case false:
                _soundSwitch.sprite = _switchSprites[1];
                _isSoundOn = true;
                break;
        }
    }
    public void TurnHapticSwitch()
    {
        switch (_isHapticOn)
        {
            case true:
                _hapticSwitch.sprite = _switchSprites[0];
                _isHapticOn = false;
                break;
            case false:
                _hapticSwitch.sprite = _switchSprites[1];
                _isHapticOn = true;
                break;
        }
    }
}
