using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private Canvas[] _earningCanvases;
    [SerializeField] private Canvas _startCanvas, _rewardCanvas;
    [SerializeField] private GameObject _levelCompleteBtn, _levelLostBtn;

    [Header("Reward Screen")]
    [SerializeField] private TextMeshProUGUI _clearPercentage;
    [SerializeField] private TextMeshProUGUI _rewardAmount;
    [SerializeField] private int _objectiveBonus = 500;
    [SerializeField] private float _timeBonusMultiplier = 100.0f;
    private string _rewardAmountString;

    [Header("UI Elements")]
    [SerializeField] private List<Sprite> _starSprites;
    [SerializeField] private Image _starImage;
    [SerializeField] private Canvas _objectivePopUp;
    [SerializeField] private TextMeshProUGUI _popupText;

    [Header("Objective Data")]
    [SerializeField] private ObjectiveData[] _objectives;
    [SerializeField] private AudioClip _objectiveSoundClip;
    [SerializeField] private float _objectivePopUpTime = 3.0f;
    public ObjectiveData[] Objectives => _objectives;
    private int _starsEarned = 0;
    private float _timeSinceStart = 0;
    private bool _isLevelComplete = false;

    // track the progress and completion status of each objective
    private Dictionary<ObjectiveType, int> _objectiveProgress = new();
    private Dictionary<ObjectiveType, bool> _objectiveCompletion = new();

    private bool[] _objectiveCompleted = new bool[2];

    [Header("Progression data")]
    [SerializeField] private int _levelID = 0;
    [SerializeField] private int _maxProgression = 0;
    [SerializeField] private int _currentProgression = 0;
    [SerializeField] private int _progressionPercentage = 0;
    [SerializeField] private float _timeLimit = 0.0f;
    public float TimeLimit => _timeLimit;

    [Header("Upgrade data")]
    [SerializeField] private float _additionalTime = 2.0f;

    private bool _hasLost = false;
    private bool _gameStarted = false;


    #region Unity Callbacks
    private void Awake()
    {
        for (int i = 0; i < _objectives.Length; i++)
        {
            _objectiveProgress[_objectives[i].ObjectiveType] = 0;
            _objectiveCompletion[_objectives[i].ObjectiveType] = false;
            Debug.Log("Set Objective" + i + ", " + _objectiveCompletion[_objectives[i].ObjectiveType]);
        }
    }
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
        _levelID = (int)_gameManager.SceneType;
        _isLevelComplete = false;
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
        _progressionPercentage = Mathf.RoundToInt(newClampedProgression * 100);
        EventManager.InvokeProgressionChange(newClampedProgression);

        CheckProgressCompletion();
    }
    public void CalculateProgressionReward()
    {
        int progressionReward = _progressionPercentage * 30;
        int totalReward = progressionReward + _starsEarned * _objectiveBonus;
        //Debug.Log($"<color=red>progressionPercentage: {progressionPercentage},\n starsEarned: {starsEarned},\n totalReward: {totalReward} </color>");
        _clearPercentage.text = _progressionPercentage.ToString() + "%";
        _rewardAmountString = totalReward.ToString();
        _rewardAmount.text = _rewardAmountString;
        EventManager.InvokeEarnCurrency(totalReward);
    }
    public void CalculateTimeBonus(float timeRemaining)
    {
        int bonus = Mathf.RoundToInt(timeRemaining * _timeBonusMultiplier);
        //Debug.Log($"<color=red>timeRemaining: {timeRemaining}, time bonus(*100): {bonus} </color>");
        EventManager.InvokeEarnCurrency(bonus);

    }
    private void CalculateStars()
    {
        for (int i = 0; i < _objectives.Length; i++)
        {
            if (_objectiveCompletion[_objectives[i].ObjectiveType])
            {
                _starsEarned++;
            }
        }

        Debug.Log("Stars Earned: " + _starsEarned);
        CalculateProgressionReward();
        ShowStars(_starsEarned);
    }
    private void ShowStars(int starsEarned) // maybe should be called SetStars 
    {
        if (starsEarned >= 0 && starsEarned < _starSprites.Count)
        {
            _starImage.sprite = _starSprites[starsEarned];
        }
    }
    private void TurnOffObjectivePopup()
    {
        _objectivePopUp.gameObject.SetActive(false);
    }

    private void CheckProgressCompletion()
    {
        if (!_isLevelComplete && _currentProgression >= _maxProgression)
        {
            _isLevelComplete = true;
            EventManager.InvokeLevelComplete();
        }
    }
    private void ShowCompletionPopup(string message)
    {
        CalculateProgressionReward();
        _popupText.text = message;
        _objectivePopUp.gameObject.SetActive(true);
        Invoke(nameof(TurnOffObjectivePopup), _objectivePopUpTime);
        SoundManager.Instance.PlayEventSound(_objectiveSoundClip);
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
        _rewardCanvas.gameObject.SetActive(true);
        _levelCompleteBtn.gameObject.SetActive(false);
        _levelLostBtn.gameObject.SetActive(true);
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
        for (int i = 0; i < _earningCanvases.Length; i++)
        {
            _earningCanvases[i].gameObject.SetActive(false);
        }

        _startCanvas.gameObject.SetActive(false);
        _gameStarted = true;
        EventManager.InvokeLevelLaunched();
    }
    public void ReloadLevel()
    {
        EventManager.InvokeReloadLevel();
        _gameManager.ChangeScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    public void ProceedToNextLevel()
    {
        _gameManager.ChangeScene();
    }

    public void UpdateObjective(ObjectiveType objectiveType)
    {
        ObjectiveData objective = Array.Find(_objectives, obj => obj.ObjectiveType == objectiveType);
        _objectiveProgress[objectiveType]++;

        //Debug.Log("another point: " + _objectiveProgress[objectiveType].ToString());

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
        CalculateTimeBonus(_timeLimit);
        _rewardCanvas.gameObject.SetActive(true);
        _levelLostBtn.gameObject.SetActive(false);
        _levelCompleteBtn.gameObject.SetActive(true);

        /* maybe add earning transition animation to the correct ui
         * for (int i = 0; i < _earningCanvases.Length; i++)
        {
            _earningCanvases[i].gameObject.SetActive(false);
        }*/
    }
    private void OnUpgrade(UpgradeType type)
    {
        if (type == UpgradeType.Time)
            ExtendTime(_additionalTime);
    }

    public void LoadData(GameData gameData)
    {
        int objectiveAmount = Enum.GetValues(typeof(ObjectiveType)).Length - 1;
        for (int i = 0; i < objectiveAmount; i++)
        {
            bool isCompleted;
            gameData.ObjectivesCompleted.TryGetValue(i, out isCompleted);
            _objectiveCompletion[(ObjectiveType)i] = isCompleted;
            Debug.Log("Load Objective" + i + ", " + _objectiveCompletion[_objectives[i].ObjectiveType]);
        }
    }
    public void SaveData(ref GameData gameData)
    {
        int objectiveAmount = Enum.GetValues(typeof(ObjectiveType)).Length - 1;
        for (int i = 0; i < objectiveAmount; i++)
        {
            if (gameData.ObjectivesCompleted.ContainsKey(i))
            {
                gameData.ObjectivesCompleted.Remove(i);
            }
            gameData.ObjectivesCompleted.Add(i, _objectiveCompletion[(ObjectiveType)i]);
        }
    }
}