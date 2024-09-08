using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneType
{
    Lobby,
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

    public void ChangeScene(SceneType nextScene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)nextScene);
    }
    public void ChangeScene(int nextSceneBuildIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneBuildIndex);
    }

    private void OnSceneChange(SceneType nextScene)
    {
        ChangeScene(nextScene);
    }
}
