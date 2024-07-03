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
    [SerializeField] private ObjectiveData[] objectives;

    // track the progress and completion status of each objective
    private Dictionary<ObjectiveType, int> _objectiveProgress = new();
    private Dictionary<ObjectiveType, bool> _objectiveCompletion = new();
    
    [Header("Progression data")]
    [SerializeField] private int _maxProgression = 0;
    [SerializeField] private int _currentProgression = 0;
    [SerializeField] private float _timeLimit = 0.0f;

    private bool _hasLost = false;

    private void OnEnable()
    {
        EventManager.OnProgressMade += OnProgressMade;
        EventManager.OnObjectiveTrigger1 += OnObjectiveTrigger1;
        EventManager.OnObjectiveTrigger2 += OnObjectiveTrigger2;
        EventManager.OnObjectiveTrigger3 += OnObjectiveTrigger3;
    }
    private void Start()
    {
        foreach (var objective in objectives)
        {
            _objectiveProgress[objective.objectiveType] = 0;
            _objectiveCompletion[objective.objectiveType] = false;
        }

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
        int starsEarned = 0;
        foreach (var objective in objectives)
        {
            if (_objectiveCompletion[objective.objectiveType])
            {
                starsEarned++;
            }
        }
        
        Debug.Log("Stars Earned: " + starsEarned);
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
        if (_hasLost)
            return;

        _timeLimit -= Time.deltaTime;
        EventManager.InvokeTimerChange(_timeLimit);

        if (_timeLimit <= 0)
            GameOver();
    }
    private void GameOver()
    {
        // do lose condition logic
        _timeLimit = 0;
        _loseCanvas.SetActive(true);
        EventManager.InvokeLose();
        _hasLost = true;
    }

    public void ReloadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void OnProgressMade(int progressToMake)
    {
        MakeProgress(progressToMake);
    }
    private void OnObjectiveTrigger1()
    {
        UpdateObjective(ObjectiveType.Objective1);
    }
    private void OnObjectiveTrigger2()
    {
        UpdateObjective(ObjectiveType.Objective2);
    }
    private void OnObjectiveTrigger3()
    {
        UpdateObjective(ObjectiveType.Objective3);
    }

    private void UpdateObjective(ObjectiveType objectiveType)
    {
        var objective = Array.Find(objectives, obj => obj.objectiveType == objectiveType);
        _objectiveProgress[objectiveType]++;
        
        if (_objectiveProgress[objectiveType] >= objective.completionCondition && !_objectiveCompletion[objectiveType])
        {
            _objectiveCompletion[objectiveType] = true;
            Debug.Log(objective.notificationText); // Will be changed to a message on screen, now just for testing
        }
    }
}
