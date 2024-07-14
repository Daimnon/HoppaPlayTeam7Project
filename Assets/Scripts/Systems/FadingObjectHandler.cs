using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingObjectHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Vector3 _playerPosOffset = Vector3.up;
    [SerializeField] private Vector3 _directionFromCamToPlayer = Vector3.forward;
    [SerializeField] private Transform _camTr;
    [SerializeField] private Transform _playerTr;
    [SerializeField] private float _targetAlpha = 0.33f;
    [SerializeField] private float _fadeSpeed = 0.5f;
    [SerializeField] private float _distanceFromPlayer = 8.0f;
    [SerializeField] private bool _retainShadows = true;

    [Header("ReadOnly Data")]
    [SerializeField] private List<FadingObject> _objectsBlockingView = new List<FadingObject>();
    private Dictionary<FadingObject, Coroutine> _runningCoroutines = new Dictionary<FadingObject, Coroutine>();

    private RaycastHit[] _hits = new RaycastHit[5];

    private void Start()
    {
        Vector3 correctPlayerOffset = _playerTr.position + _playerPosOffset;
        _distanceFromPlayer = Vector3.Distance(_camTr.position, correctPlayerOffset);
        _directionFromCamToPlayer = (correctPlayerOffset - _camTr.position).normalized;
        StartCoroutine(CheckForObjects());
    }

    private IEnumerator CheckForObjects()
    {
        while (true)
        {
            int hits = Physics.RaycastNonAlloc(_camTr.position, _directionFromCamToPlayer, _hits, _distanceFromPlayer, _layerMask);

            if (hits > 0)
            {
                for (int i = 0; i < hits; i++) 
                { 
                    FadingObject fadingObject = GetFadingObjectFromHit(_hits[i]);
                    if (fadingObject && !_objectsBlockingView.Contains(fadingObject))
                    {
                        if (_runningCoroutines.ContainsKey(fadingObject))
                        {
                            Coroutine coroutine = _runningCoroutines[fadingObject];
                            if (coroutine != null)
                                StopCoroutine(coroutine);

                            _runningCoroutines.Remove(fadingObject);
                        }

                        _runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                        _objectsBlockingView.Add(fadingObject);
                    }
                }
            }

            FadeObjectsNoLongerBeingHit();
            ClearHits();

            yield return null;
        }
    }


    private FadingObject GetFadingObjectFromHit(RaycastHit hit)
    {
        return hit.collider != null ? hit.collider.GetComponent<FadingObject>() : null;
    }
    private IEnumerator FadeObjectOut(FadingObject fadingObject)
    {
        foreach (Material material in fadingObject.Materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.SetInt("_Surface", 1);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            material.SetShaderPassEnabled("DepthOnly", false);
            material.SetShaderPassEnabled("SHADOWCASTER", _retainShadows);

            material.SetOverrideTag("RenderType", "Transparent");

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.EnableKeyword("ALPHAPREMULTIPLY_ON");
        }

        float time = 0;

        while (fadingObject.Materials[0].color.a > _targetAlpha)
        {
            for (int i = 0; i < fadingObject.Materials.Count; i++)
            {
                Material material = fadingObject.Materials[i];
                if (material.HasProperty("_Color"))
                {
                    float currentAlpha = material.color.a;
                    currentAlpha = Mathf.Lerp(currentAlpha, _targetAlpha, time * _fadeSpeed);
                    material.color = new Color(material.color.r, material.color.g, material.color.b, currentAlpha);
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        RemoveCoroutine(fadingObject);
    }

    private IEnumerator FadeObjectIn(FadingObject fadingObject)
    {
        float time = 0;

        while (fadingObject.Materials[0].color.a < fadingObject.OriginalAlpha[0])
        {
            for (int i = 0; i < fadingObject.Materials.Count; i++)
            {
                Material material = fadingObject.Materials[i];
                if (material.HasProperty("_Color"))
                {
                    float currentAlpha = material.color.a;
                    currentAlpha = Mathf.Lerp(currentAlpha, fadingObject.OriginalAlpha[i], time * _fadeSpeed);
                    material.color = new Color(material.color.r, material.color.g, material.color.b, currentAlpha);
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (Material material in fadingObject.Materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.SetInt("_Surface", 0);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            material.SetShaderPassEnabled("DepthOnly", true);
            material.SetShaderPassEnabled("SHADOWCASTER", true);

            material.SetOverrideTag("RenderType", "Opaque");

            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("ALPHAPREMULTIPLY_ON");
        }

        RemoveCoroutine(fadingObject);
    }
    private void FadeObjectsNoLongerBeingHit()
    {
        List<FadingObject> objectsToRemove = new (_objectsBlockingView.Count);

        foreach (FadingObject fadingObject in _objectsBlockingView)
        {
            bool isObjectBeingHit = false;
            for (int i = 0; i < _hits.Length; i++)
            {
                FadingObject hitFadingObject = GetFadingObjectFromHit(_hits[i]);
                if (hitFadingObject && fadingObject == hitFadingObject)
                {
                    isObjectBeingHit = true;
                    break;
                }
            }

            if (!isObjectBeingHit)
            {
                if (_runningCoroutines.ContainsKey(fadingObject))
                {
                    if (_runningCoroutines[fadingObject] != null)
                        StopCoroutine(_runningCoroutines[fadingObject]);

                    _runningCoroutines.Remove(fadingObject);
                }

                _runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectIn(fadingObject)));
                objectsToRemove.Add(fadingObject);
            }
        }

        foreach (FadingObject fadingObjectToRemove in objectsToRemove)
        {
            _objectsBlockingView.Remove(fadingObjectToRemove);
        }
    }
    private void ClearHits()
    {
        Array.Clear(_hits, 0, _hits.Length);
    }
    private void RemoveCoroutine(FadingObject fadingObject)
    {
        if (_runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(_runningCoroutines[fadingObject]);
            _runningCoroutines.Remove(fadingObject);
        }
    }
}
