using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum SwitchType 
{
    Sound,
    Haptic
}

public class Pause_Menu : MonoBehaviour
{
    [SerializeField] private GameObject[] _windowsToTurnOff; // 0 = off, 1 = on.
    [SerializeField] private Sprite[] _switchSprites; // 0 = off, 1 = on.

    [SerializeField] private Image _soundSwitch;
    [SerializeField] private Image _hapticSwitch;

    [SerializeField] private RectTransform _soundSwitchHandle;
    [SerializeField] private RectTransform _hapticSwitchHandle;

    [SerializeField] private Transform[] _soundSwitchHandleTrs;
    [SerializeField] private Transform[] _hapticSwitchHandleTrs;

    [SerializeField] private float _switchAnimationDuration = 0.2f;

    private Coroutine _soundSwitchRoutine;
    private Coroutine _hapticSwitchRoutine;
    private bool[] _tempOriginalWindowsActive;

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

    private IEnumerator AnimateSoundSwitchOffRoutine()
    {
        SoundManager.Instance.TurnOffSounds();
        float elapsedTime = 0f;
        Vector3 _initialSoundHandlePosition = _soundSwitchHandle.position;

        while (elapsedTime < _switchAnimationDuration)
        {
            float t = elapsedTime / _switchAnimationDuration;

            if (t >= 0.5f)
                _soundSwitch.sprite = _switchSprites[0]; 

            _soundSwitchHandle.position = Vector3.Lerp(_initialSoundHandlePosition, _soundSwitchHandleTrs[0].position, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _soundSwitch.sprite = _switchSprites[0];
        _soundSwitchHandle.position = _soundSwitchHandleTrs[0].position;
        _soundSwitchRoutine = null;
    }
    private IEnumerator AnimateSoundSwitchOnRoutine()
    {
        SoundManager.Instance.TurnOnSounds();
        float elapsedTime = 0f;
        Vector3 _initialSoundHandlePosition = _soundSwitchHandle.position;

        while (elapsedTime < _switchAnimationDuration)
        {
            float t = elapsedTime / _switchAnimationDuration;

            if (t >= 0.5f)
                _soundSwitch.sprite = _switchSprites[1];

            _soundSwitchHandle.position = Vector3.Lerp(_initialSoundHandlePosition, _soundSwitchHandleTrs[1].position, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _soundSwitch.sprite = _switchSprites[1];
        _soundSwitchHandle.position = _soundSwitchHandleTrs[1].position;
        _soundSwitchRoutine = null;
    }

    private IEnumerator AnimateHapticSwitchOffRoutine()
    {
        SoundManager.Instance.TurnOffHaptic();
        float elapsedTime = 0f;
        Vector3 _initialHapticHandlePosition = _hapticSwitchHandle.position;

        while (elapsedTime < _switchAnimationDuration)
        {
            float t = elapsedTime / _switchAnimationDuration;

            if (t >= 0.5f)
                _hapticSwitch.sprite = _switchSprites[0];

            _hapticSwitchHandle.position = Vector3.Lerp(_initialHapticHandlePosition, _hapticSwitchHandleTrs[0].position, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _hapticSwitch.sprite = _switchSprites[0];
        _hapticSwitchHandle.position = _hapticSwitchHandleTrs[0].position;
        _hapticSwitchRoutine = null;
    }
    private IEnumerator AnimateHapticSwitchOnRoutine()
    {
        SoundManager.Instance.TurnOnHaptic();
        float elapsedTime = 0f;
        Vector3 _initialHapticHandlePosition = _hapticSwitchHandle.position;

        while (elapsedTime < _switchAnimationDuration)
        {
            float t = elapsedTime / _switchAnimationDuration;

            if (t >= 0.5f)
                _hapticSwitch.sprite = _switchSprites[1];

            _hapticSwitchHandle.position = Vector3.Lerp(_initialHapticHandlePosition, _hapticSwitchHandleTrs[1].position, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _hapticSwitch.sprite = _switchSprites[1];
        _hapticSwitchHandle.position = _hapticSwitchHandleTrs[1].position;
        _hapticSwitchRoutine = null;
    }

    public void TurnSoundSwitch()
    {
        if (_soundSwitchRoutine != null) return;
        switch (_isSoundOn)
        {
            case true:
                _soundSwitch.sprite = _switchSprites[0];
                _isSoundOn = false;
                _soundSwitchRoutine = StartCoroutine(AnimateSoundSwitchOffRoutine());
                break;
            case false:
                _soundSwitch.sprite = _switchSprites[1];
                _soundSwitchRoutine = StartCoroutine(AnimateSoundSwitchOnRoutine());
                _isSoundOn = true;
                break;
        }
    }
    public void TurnHapticSwitch()
    {
        if (_hapticSwitchRoutine != null) return;
        switch (_isHapticOn)
        {
            case true:
                _isHapticOn = false;
                _hapticSwitchRoutine = StartCoroutine(AnimateHapticSwitchOffRoutine());
                break;
            case false:
                _isHapticOn = true;
                _hapticSwitchRoutine = StartCoroutine(AnimateHapticSwitchOnRoutine());
                break;
        }
    }
}
