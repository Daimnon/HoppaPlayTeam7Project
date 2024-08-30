using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject _startCanvas;

    [Header("UI Elements")]
    [SerializeField] private List<Sprite> _starSprites;
    [SerializeField] private Image _starImage;
    [SerializeField] private GameObject _completionPopup;
    [SerializeField] private TMPro.TextMeshProUGUI _popupText;

    [Header("Objective Data")]
    [SerializeField] private ObjectiveData[] _objectives;
    public ObjectiveData[] Objectives => _objectives;


    // track the progress and completion status of each objective
    private Dictionary<ObjectiveType, int> _objectiveProgress = new();
    private Dictionary<ObjectiveType, bool> _objectiveCompletion = new();
    
    [Header("Progression data")]
    [SerializeField] private int _maxProgression = 0;
    [SerializeField] private int _currentProgression = 0;
    [SerializeField] private float _timeLimit = 0.0f;
    public float TimeLimit => _timeLimit;

    private SoundManager soundManager;
    private bool _hasLost = false;
    private bool _gameStarted = false;

    private void OnEnable()
    {
        EventManager.OnProgressMade += OnProgressMade;
        EventManager.OnObjectiveTrigger1 += OnObjectiveTrigger1;
        EventManager.OnObjectiveTrigger2 += OnObjectiveTrigger2;
        EventManager.OnObjectiveTrigger3 += OnObjectiveTrigger3;
    }
    private void Start()
    {
        soundManager = FindAnyObjectByType<SoundManager>();

        foreach (var objective in _objectives)
        {
            _objectiveProgress[objective.ObjectiveType] = 0;
            _objectiveCompletion[objective.ObjectiveType] = false;
        }
    }
    private void Update()
    {
        if (!_gameStarted)
            return;

        HandleLoseCondition(); // considering to change for coroutine, more performant - need testing?
    }
    private void OnDisable()
    {
        EventManager.OnProgressMade -= OnProgressMade;
        EventManager.OnObjectiveTrigger1 -= OnObjectiveTrigger1;
        EventManager.OnObjectiveTrigger2 -= OnObjectiveTrigger2;
        EventManager.OnObjectiveTrigger3 -= OnObjectiveTrigger3;
    }

    public void StartGame()
    {
        _startCanvas.SetActive(false);
        _gameStarted = true;
        EventManager.InvokeLevelLaunched();
    }

    private void CalculateStars()
    {
        int starsEarned = 0;
        foreach (var objective in _objectives)
        {
            if (_objectiveCompletion[objective.ObjectiveType])
            {
                starsEarned++;
            }
        }
        
        Debug.Log("Stars Earned: " + starsEarned);
        ShowStars(starsEarned);
    }

    private void ShowStars(int starsEarned)
    {
        if (starsEarned >= 0 && starsEarned < _starSprites.Count)
        {
            _starImage.sprite = _starSprites[starsEarned];
        }
    }

    private void CheckProgressCompletion()
    {
        if (_currentProgression >= _maxProgression)
            CalculateStars();
    }
    public void MakeProgress(int progressToMake)
    {
        _currentProgression += progressToMake;
        float newClampedProgression = Mathf.Clamp01((float)_currentProgression / _maxProgression);
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
        CalculateStars();
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

    public void UpdateObjective(ObjectiveType objectiveType)
    {
        var objective = Array.Find(_objectives, obj => obj.ObjectiveType == objectiveType);
        _objectiveProgress[objectiveType]++;

        Debug.Log("another point: " + _objectiveProgress[objectiveType].ToString());
        
        if (_objectiveProgress[objectiveType] >= objective.CompletionCondition && !_objectiveCompletion[objectiveType])
        {
            _objectiveCompletion[objectiveType] = true;
            ShowCompletionPopup(objective.NotificationText);
            Debug.Log(objective.NotificationText); // Will be changed to a message on screen, now just for testing
        }
    }

    private void ShowCompletionPopup(string message)
    {
        _popupText.text = message;
        _completionPopup.SetActive(true);
        soundManager.PlayCatSound();
        StartCoroutine(HideCompletionPopup());
    }

    private IEnumerator HideCompletionPopup()
    {
        yield return new WaitForSeconds(2.0f);
        _completionPopup.SetActive(false);
    }

    internal void ExtendTime(int additionalTime)
    {
        _timeLimit += additionalTime;
        EventManager.InvokeTimerChange(_timeLimit);
    }
}
