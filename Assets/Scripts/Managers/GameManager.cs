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

public class GameManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.OnSceneChange += OnSceneChange;
    }
    private void Start()
    {
        EventManager.InvokeGameLaunched();
    }
    private void OnDisable()
    {
        EventManager.OnSceneChange -= OnSceneChange;
    }

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
}
