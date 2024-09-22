using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;
    public static SaveManager Instance => _instance;

    private GameData _gameData;
    private List<ISaveable> _saveables;

    [Header("File Storage Config")]
    [SerializeField] private string _fileName;
    private FileDataHandler _fileDataHandler;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this);
        else
            _instance = this;
    }
    private void Start()
    {
        EventManager.OnLevelLaunched += OnLevelLaunched;
        EventManager.OnLevelComplete += OnLevelComplete;
        EventManager.OnReloadLevel += OnReloadLevel;
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath + "/", _fileName);
        Debug.Log(Application.persistentDataPath);
        _saveables = FindAllSaveables();
        LoadGame();
    }
    private void OnDestroy()
    {
        EventManager.OnLevelLaunched -= OnLevelLaunched;
        EventManager.OnLevelComplete -= OnLevelComplete;
        EventManager.OnReloadLevel -= OnReloadLevel;
    }

    public void NewGame()
    {
        _gameData = new GameData();
    }
    public void LoadGame()
    {
        _gameData = _fileDataHandler.Load();

        if (_gameData == null)
        {
            Debug.Log("No previous data, create new save file.");
            NewGame();
        }

        for (int i = 0; i < _saveables.Count; i++)
        {
            _saveables[i].LoadData(_gameData);
        }

        EventManager.InvokeTimerChange(_gameData.TimeLimit);
        EventManager.InvokeCurrencyChange(_gameData.Currency);
        EventManager.InvokeSpecialCurrencyChange(_gameData.SpecialCurrency);
    }
    public void SaveGame()
    {
        for (int i = 0; i < _saveables.Count; i++)
        {
            _saveables[i].SaveData(ref _gameData);
        }

        _fileDataHandler.Save(_gameData);
    }

    private List<ISaveable> FindAllSaveables()
    {
        IEnumerable<ISaveable> saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        return new List<ISaveable>(saveables);
    }

    private void OnLevelLaunched()
    {
        SaveGame();
    }
    private void OnReloadLevel()
    {
        SaveGame();
        LoadGame();
    }
    private void OnLevelComplete()
    {
        _gameData.LevelID++;
        _gameData.IsNewLevel = true;

        SaveGame();
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
