using System;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance => _instance;

    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _uiSource;
    [SerializeField] private AudioSource _playerSource;
    [SerializeField] private AudioSource _eventSource;
    [SerializeField] private AudioSource _endGameSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip _menuMusicClip;
    [SerializeField] private AudioClip _levelMusicClip;

    [Header("UI Clips")]
    [SerializeField] private AudioClip _earnCurrencyClip;
    [SerializeField] private AudioClip _earnSpecialClip;
    [SerializeField] private AudioClip _purchaseClip;
    [SerializeField] private AudioClip _purchaseSpecialClip;
    [SerializeField] private AudioClip _offerClip;
    [SerializeField] private AudioClip _equipClip;
    [SerializeField] private AudioClip _playClip;
    [SerializeField] private AudioClip _pauseClip;

    [Header("Event Clips")]
    [SerializeField] private AudioClip _levelUpClip;
    [SerializeField] private AudioClip _explosionClip;
    [SerializeField] private AudioClip _lastCallClip;

    [Header("EndGame Clips")]
    [SerializeField] private AudioClip _levelCompleteClip;
    [SerializeField] private AudioClip _gameOverClip;

    [Header("Data")]
    [SerializeField] private Vector2 _pitchRange = new(0.5f, 1.5f);
    private bool _shouldVibrate = true;

    private void Awake()
    {
        _instance = this;
    }
    private void OnEnable()
    {
        EventManager.OnLose += OnLose;
        EventManager.OnGrowth += OnGrowth;
        EventManager.OnAreaClosed += OnAreaClosed;
        EventManager.OnPayCurrency += OnPayCurrency;
    }
    private void Start()
    {
        PlayMusic(_menuMusicClip);
    }   
    private void OnDisable()
    {
        EventManager.OnLose -= OnLose;
        EventManager.OnGrowth -= OnGrowth;
        EventManager.OnAreaClosed -= OnAreaClosed;
        EventManager.OnPayCurrency -= OnPayCurrency;
    }
        
    private void PlayOneShot(AudioSource source, AudioClip clip)
    {
        if (_shouldVibrate)
            Handheld.Vibrate();

        source.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", UnityEngine.Random.Range(_pitchRange.x, _pitchRange.y));
        source.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        _musicSource.Play();
    }
    public void PlayUISound(AudioClip clip)
    {
        PlayOneShot(_uiSource, clip);
    }
    public void PlayPlayerSound(AudioClip clip)
    {
        PlayOneShot(_playerSource, clip);
    }
    public void PlayEventSound(AudioClip clip)
    {
        PlayOneShot(_eventSource, clip);
    }
    public void PlayEndGame(AudioClip clip)
    {
        PlayOneShot(_endGameSource, clip);
    }

    public void TurnOffSounds()
    {
        _musicSource.mute = true;
        _uiSource.mute = true;
        _playerSource.mute = true; 
        _eventSource.mute = true;
        _endGameSource.mute = true;
    }
    public void TurnOnSounds()
    {
        _musicSource.mute = false;
        _uiSource.mute = false;
        _playerSource.mute = false;
        _eventSource.mute = false;
        _endGameSource.mute = false;
    }

    public void TurnOnHaptic()
    {
        _shouldVibrate = true;
    }
    public void TurnOffHaptic()
    {
        _shouldVibrate = false;
    }

    private void OnGrowth()
    {
        PlayOneShot(_eventSource, _levelUpClip);
    }
    private void OnLose()
    {
        PlayOneShot(_endGameSource, _gameOverClip);
    }
    private void OnAreaClosed(Vector3 midPos)
    {
        PlayEventSound(_explosionClip);

    }
    private void OnPayCurrency(int amount)
    {
        PlayEventSound(_purchaseClip);
    }
}
