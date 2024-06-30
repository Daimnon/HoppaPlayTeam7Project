using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Progression data")]
    [SerializeField] private int _maxProgression = 0;
    [SerializeField] private int _currentProgression = 0;
    [SerializeField] private float _timeSinceStartLevel = 0.0f;

    private void OnEnable()
    {
        EventManager.OnProgressMade += OnProgressMade;
    }
    private void Start()
    {
        EventManager.InvokeLevelLaunched();
    }
    private void Update()
    {
        _timeSinceStartLevel = Time.timeSinceLevelLoad;
        EventManager.InvokeTimerChange(_timeSinceStartLevel);
    }
    private void OnDisable()
    {
        EventManager.OnProgressMade -= OnProgressMade;
    }

    private void CompleteLevel()
    {
        // do completion logic
    }
    private void CheckProgressCompletion()
    {
        if (_currentProgression >= _maxProgression)
            CompleteLevel();
    }
    public void MakeProgress(int progressToMake)
    {
        _currentProgression += progressToMake;
        float newClampedProgression = Mathf.Clamp01((float)_currentProgression / _maxProgression); // currentValue is float for correct clamp in fillAmount
        EventManager.InvokeProgressionChange(newClampedProgression);

        CheckProgressCompletion();
    }
    private void OnProgressMade(int progressToMake)
    {
        MakeProgress(progressToMake);
    }
}
