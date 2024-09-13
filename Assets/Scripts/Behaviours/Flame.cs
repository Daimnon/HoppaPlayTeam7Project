using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    [SerializeField] float _explosionDuration = 2f;
    [SerializeField] float _explosionSlowDownPoint = 0.8f;
    [SerializeField] float _timeToDieOut = 3.0f;
    private Vector3 _initialScale;

    private void Start()
    {
        _initialScale = transform.localScale;
    }

    private IEnumerator GrowFlameRoutine(Vector3 targetScale)
    {
        transform.localScale = Vector3.zero;

        float elapsedTime = 0f;
        Vector3 startingScale = Vector3.zero;

        while (elapsedTime < _explosionDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (_explosionDuration / 2);

            float growFactor = Mathf.SmoothStep(0f, 1f, t);
            transform.localScale = Vector3.Lerp(startingScale, targetScale, growFactor);

            yield return null;
        }
        transform.localScale = targetScale;
    }
    private IEnumerator ExplosionRoutine(Vector3 targetExplosionScale)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _explosionDuration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the percentage of completion
            float t = elapsedTime / _explosionDuration;

            if (t < _explosionSlowDownPoint)
            {
                // Fast growth phase (exponential or linear)
                float fastGrowthFactor = Mathf.Pow(t / _explosionSlowDownPoint, 0.5f); // Adjust the exponent for faster or slower growth
                transform.localScale = Vector3.Lerp(_initialScale, targetExplosionScale * _explosionSlowDownPoint, fastGrowthFactor);
            }
            else
            {
                // Slow growth phase (ease out)
                float slowDownFactor = Mathf.SmoothStep(_explosionSlowDownPoint, 1f, (t - _explosionSlowDownPoint) / (1f - _explosionSlowDownPoint));
                transform.localScale = Vector3.Lerp(_initialScale * _explosionSlowDownPoint, targetExplosionScale, slowDownFactor);
            }

            yield return null; // Wait for the next frame
        }

        // Ensure the object reaches the target scale exactly
        transform.localScale = targetExplosionScale;        
    }
    private IEnumerator EndExplosionRoutine()
    {
        float elapsedTime = 0f;
        Vector3 startingScale = transform.localScale;

        while (elapsedTime < _explosionDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (_explosionDuration / 2);

            float shrinkFactor = Mathf.SmoothStep(0f, 1f, t);
            transform.localScale = Vector3.Lerp(startingScale, Vector3.zero, shrinkFactor);

            yield return null;
        }
        transform.localScale = Vector3.zero;
    }

    public void GrowFlame(Vector3 targetScale)
    {
        StartCoroutine(GrowFlameRoutine(targetScale));
    }
    public void DoExplosion(Vector3 targetExplosionScale)
    {
        StartCoroutine(ExplosionRoutine(targetExplosionScale));
    }
    public Coroutine EndExplosion()
    {
        return StartCoroutine(EndExplosionRoutine());
    }
    public void ResetExplosion()
    {
        gameObject.SetActive(false);
        transform.localScale = _initialScale;
    }
}
