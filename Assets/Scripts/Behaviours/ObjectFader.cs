using System.Collections;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    [SerializeField] private Material _mat;
    [SerializeField] private float _targetOpacity;
    [SerializeField] private float _fadeSpeed;

    [SerializeField] private bool _shouldFade;
    private Material _originalMat;
    private Material _tempMat;

    public bool ShouldFade
    {
        get => _shouldFade;
        set
        {
            _shouldFade = value;
            if (_shouldFade)
            {
                StopAllCoroutines();
                StartCoroutine(FadeTo(_targetOpacity));
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(FadeTo(_originalOpacity));
            }
        }
    }

    private float _originalOpacity;

    private void Start()
    {
        _originalOpacity = _mat.color.a;
        _originalMat = _mat;
        _tempMat = new Material(_mat);
    }

    private IEnumerator FadeTo(float targetOpacity)
    {
        while (!Mathf.Approximately(_mat.color.a, targetOpacity))
        {
            Color currentColor = _mat.color;
            currentColor.a = Mathf.Lerp(currentColor.a, targetOpacity, _fadeSpeed * Time.deltaTime);
            _mat.color = currentColor;
            yield return null;
        }
    }
}
