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
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileName);
        _saveables = FindAllSaveables();
        LoadGame();
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

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
