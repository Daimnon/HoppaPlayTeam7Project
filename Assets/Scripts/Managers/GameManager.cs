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
    [SerializeField] private SceneType _sceneType = SceneType.Level1;
    [SerializeField] private Scene _loadingScene;

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

/*    private IEnumerator ChangeSceneWithLoadingScreen()
    {
        SceneManager.Load
    }*/

    /// <summary>
    /// change scene to next scene in build order
    /// </summary>
    public void ChangeScene()
    {
        Debug.Log("Changing scene, next scene");
        int nextSceneBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (SceneManager.GetSceneByBuildIndex(nextSceneBuildIndex) != null)
            SceneManager.LoadScene(nextSceneBuildIndex);
        else
            SceneManager.LoadScene(0);
    }
    public void ChangeScene(SceneType nextSceneType)
    {
        Debug.Log("Changing scene, next scene" + nextSceneType.ToString());
        int sceneBuildIndex = (int)nextSceneType;

        if (SceneManager.GetSceneByBuildIndex(sceneBuildIndex) != null)
            SceneManager.LoadScene(sceneBuildIndex);
        else
            Debug.LogError("Scene in build index " + sceneBuildIndex + " does not exist!");
    }
    public void ChangeScene(int sceneBuildIndex)
    {
        Debug.Log("Changing scene, next scene" + ((SceneType)sceneBuildIndex).ToString());
        if (SceneManager.GetSceneByBuildIndex(sceneBuildIndex) != null)
            SceneManager.LoadScene(sceneBuildIndex);
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
        /*if ((int)_sceneType != gameData.LevelID) // need to fix
            ChangeScene(gameData.LevelID);*/
    }

    public void SaveData(ref GameData gameData)
    {
    }
}
