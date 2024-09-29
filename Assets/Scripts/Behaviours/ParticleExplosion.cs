using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleExplosion : MonoBehaviour
{
    [SerializeField] ParticleSystem[] _particlseStartSizeValue;
    [SerializeField] ParticleSystem[] _particlseStartSizeRange;
    [SerializeField] float _explosionDuration = 2f;
    [SerializeField] float _explosionSlowDownPoint = 0.8f;
    private Vector3 _initialScale;

    private void Start()
    {
        _initialScale = transform.localScale;
    }

    private IEnumerator ExplosionRoutine(Vector3 targetExplosionScale, ParticleSystem[] particleArray1, ParticleSystem[] particleArray2)
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

                // Set startSize as float for first particle array
                for (int i = 0; i < particleArray1.Length; i++)
                {
                    ParticleSystem.MainModule mainModule = particleArray1[i].main;
                    mainModule.startSize = Mathf.Lerp(0f, targetExplosionScale.magnitude * _explosionSlowDownPoint, fastGrowthFactor);
                }

                // Set startSize using MinMaxCurve for second particle array
                for (int i = 0; i < particleArray2.Length; i++)
                {
                    ParticleSystem.MainModule mainModule = particleArray2[i].main;
                    float minCurveValue = Mathf.Lerp(0f, targetExplosionScale.magnitude * _explosionSlowDownPoint, fastGrowthFactor);
                    float maxCurveValue = Mathf.Lerp(0f, targetExplosionScale.magnitude * _explosionSlowDownPoint * 1.5f, fastGrowthFactor);
                    mainModule.startSize = new ParticleSystem.MinMaxCurve(minCurveValue, maxCurveValue);
                }
            }
            else
            {
                // Slow growth phase (ease out)
                float slowDownFactor = Mathf.SmoothStep(_explosionSlowDownPoint, 1f, (t - _explosionSlowDownPoint) / (1f - _explosionSlowDownPoint));

                // Set startSize as float for first particle array
                foreach (var particle in particleArray1)
                {
                    var mainModule = particle.main;
                    mainModule.startSize = Mathf.Lerp(targetExplosionScale.magnitude * _explosionSlowDownPoint, targetExplosionScale.magnitude, slowDownFactor);
                }

                // Set startSize using MinMaxCurve for second particle array
                foreach (var particle in particleArray2)
                {
                    var mainModule = particle.main;
                    float minCurveValue = Mathf.Lerp(targetExplosionScale.magnitude * _explosionSlowDownPoint, targetExplosionScale.magnitude, slowDownFactor);
                    float maxCurveValue = Mathf.Lerp(targetExplosionScale.magnitude * _explosionSlowDownPoint * 1.5f, targetExplosionScale.magnitude * 1.5f, slowDownFactor);
                    mainModule.startSize = new ParticleSystem.MinMaxCurve(minCurveValue, maxCurveValue);
                }
            }

            yield return null; // Wait for the next frame
        }

        // Ensure the particle systems reach the target size exactly
        foreach (var particle in particleArray1)
        {
            var mainModule = particle.main;
            mainModule.startSize = targetExplosionScale.magnitude;
        }

        foreach (var particle in particleArray2)
        {
            var mainModule = particle.main;
            mainModule.startSize = new ParticleSystem.MinMaxCurve(targetExplosionScale.magnitude, targetExplosionScale.magnitude * 1.5f);
        }
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

    public void DoExplosion(Vector3 targetExplosionScale)
    {
        StartCoroutine(ExplosionRoutine(targetExplosionScale, _particlseStartSizeValue, _particlseStartSizeRange));
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
