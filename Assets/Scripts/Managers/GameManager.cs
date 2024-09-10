using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ChangeScene(SceneType nextScene)
    {
        Debug.Log("Changing scene, next scene" + nextScene.ToString());
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)nextScene);
    }
    public void ChangeScene(int nextSceneBuildIndex)
    {
        Debug.Log("Changing scene, next scene" + ((SceneType)nextSceneBuildIndex).ToString());
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneBuildIndex);
    }

    private void OnSceneChange(SceneType nextScene)
    {
        ChangeScene(nextScene);
    }
}
