using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Level1,
    Level2,
    Level3,
}

public class GameManager : MonoBehaviour, ISaveable
{
    [SerializeField] private ASyncLoader _sceneLoader;
    [SerializeField] private SceneType _sceneType = SceneType.Level1;
    public SceneType SceneType => _sceneType;

    private void OnEnable()
    {
        EventManager.OnSceneChange += OnSceneChange;
        EventManager.OnLevelComplete += OnLevelComplete;
    }
    private void Start()
    {
        EventManager.InvokeGameLaunched();
    }
    private void OnDisable()
    {
        EventManager.OnSceneChange -= OnSceneChange;
        EventManager.OnLevelComplete -= OnLevelComplete;
    }

    /// <summary>
    /// change scene to next scene in build order.
    /// </summary>
    public void ChangeScene()
    {
        Debug.Log("Changing scene, next scene");
        int nextSceneBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (SceneManager.GetSceneByBuildIndex(nextSceneBuildIndex) != null)
            _sceneLoader.LoadLevel(nextSceneBuildIndex);
        else
            _sceneLoader.LoadLevel(0);
    }

    /// <summary>
    /// change scene a scene in build order using SceneType.
    /// </summary>
    public void ChangeScene(SceneType nextSceneType)
    {
        Debug.Log("Changing scene, next scene" + nextSceneType.ToString());
        int sceneBuildIndex = (int)nextSceneType;

        if (SceneManager.GetSceneByBuildIndex(sceneBuildIndex) != null)
            _sceneLoader.LoadLevel(sceneBuildIndex);
        else
            Debug.LogError("Scene in build index " + sceneBuildIndex + " does not exist!");
    }

    /// <summary>
    /// change scene a scene in build order using int.
    /// </summary>
    public void ChangeScene(int sceneBuildIndex)
    {
        Debug.Log("Changing scene, next scene" + ((SceneType)sceneBuildIndex).ToString());
        if (SceneManager.GetSceneByBuildIndex(sceneBuildIndex) != null)
            _sceneLoader.LoadLevel(sceneBuildIndex);
        else
            Debug.LogError("Scene in build index " + sceneBuildIndex + " does not exist!");
    }

    private void OnSceneChange(SceneType nextScene)
    {
        ChangeScene(nextScene);
    }
    private void OnLevelComplete()
    {
        SaveManager.Instance.SaveGame();
    }

    public void LoadData(GameData gameData)
    {
        if ((int)_sceneType != gameData.LevelID) // need to fix
            ChangeScene(gameData.LevelID);
    }
    public void SaveData(ref GameData gameData)
    {
    }
}
