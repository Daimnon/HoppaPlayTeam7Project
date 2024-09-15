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

public class LevelManager : MonoBehaviour, ISaveable
{
    [Header("Components")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private Canvas _startCanvas, _completionCanvas, _loseCanvas;

    [Header("UI Elements")]
    [SerializeField] private List<Sprite> _starSprites;
    [SerializeField] private Image _starImage;
    [SerializeField] private Canvas _objectivePopUp;
    [SerializeField] private TMPro.TextMeshProUGUI _popupText;

    [Header("Objective Data")]
    [SerializeField] private ObjectiveData[] _objectives;
    public ObjectiveData[] Objectives => _objectives;

    private float _timeSinceStart = 0;

    // track the progress and completion status of each objective
    private Dictionary<ObjectiveType, int> _objectiveProgress = new();
    private Dictionary<ObjectiveType, bool> _objectiveCompletion = new();
    
    [Header("Progression data")]
    [SerializeField] private int _levelID = 0;
    [SerializeField] private int _maxProgression = 0;
    [SerializeField] private int _currentProgression = 0;
    [SerializeField] private float _timeLimit = 0.0f;
    public float TimeLimit => _timeLimit;

    [Header("Upgrade data")]
    [SerializeField] private float _additionalTime = 2.0f;

    private bool _hasLost = false;
    private bool _gameStarted = false;

    #region Unity Callbacks
    private void OnEnable()
    {
        EventManager.OnProgressMade += OnProgressMade;
        EventManager.OnObjectiveTrigger1 += OnObjectiveTrigger1;
        EventManager.OnObjectiveTrigger2 += OnObjectiveTrigger2;
        EventManager.OnObjectiveTrigger3 += OnObjectiveTrigger3;
        EventManager.OnLevelComplete += OnLevelComplete;
        EventManager.OnUpgrade += OnUpgrade;
    }
    private void Start()
    {
        if (!_soundManager)
            _soundManager = FindAnyObjectByType<SoundManager>();

        foreach (var objective in _objectives)
        {
            _objectiveProgress[objective.ObjectiveType] = 0;
            _objectiveCompletion[objective.ObjectiveType] = false;
        }

        _levelID = (int)_gameManager.SceneType;
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
        EventManager.OnLevelComplete -= OnLevelComplete;
        EventManager.OnUpgrade -= OnUpgrade;
    }
    #endregion

    private void MakeProgress(int progressToMake)
    {
        _currentProgression += progressToMake;
        float newClampedProgression = Mathf.Clamp01((float)_currentProgression / _maxProgression);
        EventManager.InvokeProgressionChange(newClampedProgression);

        CheckProgressCompletion();
    }
    private void CalculateStars()
    {
        int starsEarned = 0;
        for (int i = 0; i < _objectives.Length; i++)
        {
            if (_objectiveCompletion[_objectives[i].ObjectiveType])
            {
                starsEarned++;
            }
        }
        
        Debug.Log("Stars Earned: " + starsEarned);
        ShowStars(starsEarned);
    }
    private void ShowStars(int starsEarned) // maybe should be called SetStars 
    {
        if (starsEarned >= 0 && starsEarned < _starSprites.Count)
        {
            _starImage.sprite = _starSprites[starsEarned];
        }
    }

    private void CheckProgressCompletion()
    {
        if (_currentProgression >= _maxProgression)
            EventManager.InvokeLevelComplete();
    }
    private void ShowCompletionPopup(string message)
    {
        _popupText.text = message;
        _objectivePopUp.gameObject.SetActive(true);
        _soundManager.PlayCatSound();
        //StartCoroutine(HideCompletionPopup());
    }
    private IEnumerator HideCompletionPopup()
    {
        yield return new WaitForSeconds(2.0f);
        _objectivePopUp.gameObject.SetActive(false);
    }

    private void HandleLoseCondition()
    {
        if (_hasLost)
        {
            EventManager.InvokeTimerChange(_timeLimit);
            return;
        }

        _timeLimit -= Time.deltaTime;
        EventManager.InvokeTimerChange(_timeLimit);

        if (_timeLimit <= 0)
            GameOver();
    }
    private void GameOver()
    {
        // do lose condition logic
        CalculateStars();
        _timeLimit = 0.0f;
        _loseCanvas.gameObject.SetActive(true);
        EventManager.InvokeLose();
        _hasLost = true;
    }

    private void ExtendTime(float additionalTime)
    {
        _timeLimit += additionalTime;
        EventManager.InvokeTimerChange(_timeLimit);
    }

    public void StartGame()
    {
        _startCanvas.gameObject.SetActive(false);
        _gameStarted = true;
        EventManager.InvokeLevelLaunched();
    }
    public void ReloadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    public void ProceedToNextLevel()
    {
        _gameManager.ChangeScene();
    }

    public void UpdateObjective(ObjectiveType objectiveType)
    {
        ObjectiveData objective = Array.Find(_objectives, obj => obj.ObjectiveType == objectiveType);
        _objectiveProgress[objectiveType]++;

        Debug.Log("another point: " + _objectiveProgress[objectiveType].ToString());
        
        if (_objectiveProgress[objectiveType] >= objective.CompletionCondition && !_objectiveCompletion[objectiveType])
        {
            _objectiveCompletion[objectiveType] = true;
            ShowCompletionPopup(objective.NotificationText);
            Debug.Log(objective.NotificationText); // Will be changed to a message on screen, now just for testing
        }
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

    private void OnLevelComplete()
    {
        CalculateStars();
        _completionCanvas.gameObject.SetActive(true);
    }
    private void OnUpgrade(UpgradeType type)
    {
        if (type == UpgradeType.Time)
            ExtendTime(_additionalTime);
    }

    public void LoadData(GameData gameData)
    {

    }
    public void SaveData(ref GameData gameData)
    {
    }
}
