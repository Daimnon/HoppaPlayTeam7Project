using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Extensions;

public class ASyncLoader : MonoBehaviour
{
    [SerializeField] private Canvas _loadingCanvas;
    [SerializeField] private Canvas _startCanvas;
    [SerializeField] private Image _loadingIcon;
    [SerializeField] private TextMeshProUGUI _loadingTextComponent;
    [SerializeField] private string _loadingTextString = "Loading";
    [SerializeField] private int _loadingTextDotSpeed = 50;
    [SerializeField] private float _iconRotationSpeed = 200.0f;
    private bool _isFireBaseInitialize = false;

    private string[] _dotStrings = new string[5] { "..", "...", "....", ".....", "......" };
    private int _dotCounter = 0;

    private void InitFirebase()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            Firebase.DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                _isFireBaseInitialize = true;
                Debug.Log($"Firebase was initialized");
            }
            else
            {
                Debug.Log($"Could not resolve all Firebase dependencies : {dependencyStatus}");
            }
        });
    }

    private void UpdateLoadingTextString()
    {
        _loadingTextComponent.text = _loadingTextString + _dotStrings[Mathf.RoundToInt(_dotCounter / _loadingTextDotSpeed)];
        _dotCounter++;

        if (_dotCounter > (_dotStrings.Length - 1) * _loadingTextDotSpeed)
            _dotCounter = 0;
    }
    private void UpdateLoadingIconRotation()
    {
        Vector3 newRotation = _loadingIcon.transform.rotation.eulerAngles;
        newRotation.z -= _iconRotationSpeed * Time.deltaTime;
        _loadingIcon.transform.rotation = Quaternion.Euler(newRotation);
    }
    private IEnumerator LoadLevelASync(int levelID)
    {
        if (!_isFireBaseInitialize) InitFirebase();
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelID);

        while (!loadOperation.isDone)
        {
            UpdateLoadingIconRotation();
            UpdateLoadingTextString();
            yield return null;
        }
    }

    /// <summary>
    /// <para>change scene to a scene in build order.
    /// <br>activate by GameManager, could use int or SceneType</br></para>
    /// </summary>
    public void LoadLevel(int levelID)
    {
        _startCanvas.gameObject.SetActive(false);
        _loadingCanvas.gameObject.SetActive(true);

        StartCoroutine(LoadLevelASync(levelID));
    }
    
}
