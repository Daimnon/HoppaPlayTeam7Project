using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveType // None should always be last, **should not expand casually as all ObjectiveTypes will reset**
{
    Objective1,
    Objective2, 
    Objective3,
    None
}

public class LevelManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject _loseCanvas;

    [Header("Objective Data")]
    [SerializeField] private string[] _objectiveNames;
    [SerializeField] private int[] _objectiveConditionByNameOrder;
    Dictionary<string, int> _objectives = new();
    
    [Header("Progression data")]
    [SerializeField] private int _maxProgression = 0;
    [SerializeField] private int _currentProgression = 0;
    [SerializeField] private float _timeLimit = 0.0f;

    private void OnEnable()
    {
        EventManager.OnProgressMade += OnProgressMade;
        EventManager.OnObjectiveTrigger1 += OnObjectiveTrigger1;
        EventManager.OnObjectiveTrigger2 += OnObjectiveTrigger2;
        EventManager.OnObjectiveTrigger3 += OnObjectiveTrigger3;
    }
    private void Start()
    {
        for (int i = 0; i < _objectives.Keys.Count; i++)
            _objectives.Add(_objectiveNames[i], _objectiveConditionByNameOrder[i]);

        EventManager.InvokeLevelLaunched();
    }
    private void Update()
    {
        HandleLoseCondition();
    }
    private void OnDisable()
    {
        EventManager.OnProgressMade -= OnProgressMade;
        EventManager.OnObjectiveTrigger1 -= OnObjectiveTrigger1;
        EventManager.OnObjectiveTrigger2 -= OnObjectiveTrigger2;
        EventManager.OnObjectiveTrigger3 -= OnObjectiveTrigger3;
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

    private void HandleLoseCondition()
    {
        _timeLimit -= Time.deltaTime;
        EventManager.InvokeTimerChange(_timeLimit);

        if (_timeLimit <= 0)
            GameOver();
    }
    private void GameOver()
    {
        // do lose condition logic
        _loseCanvas.SetActive(true);
    }

    public int GetCondition(string objectiveName)
    {
        _objectives.TryGetValue(objectiveName, out int condition);
        return condition;
    }

    private void OnProgressMade(int progressToMake)
    {
        MakeProgress(progressToMake);
    }
    private void OnObjectiveTrigger1()
    {

    }
    private void OnObjectiveTrigger2()
    {

    }
    private void OnObjectiveTrigger3()
    {

    }
}
