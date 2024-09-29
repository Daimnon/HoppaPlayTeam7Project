using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleExplosion : MonoBehaviour
{
    [SerializeField] ParticleSystem[] _particlseStartSizeValue;
    [SerializeField] ParticleSystem[] _particlseStartSizeRange;
    [SerializeField] float _explosionDuration = 2f;
    [SerializeField] float _explosionSlowDownPoint = 0.8f;
    private float[] _initialValuesScale;
    private Vector2[] _initialRangesScale;

    private void Start()
    {
        _initialValuesScale = new float[_particlseStartSizeValue.Length];
        _initialRangesScale = new Vector2[_particlseStartSizeRange.Length];
        for (int i = 0; i < _particlseStartSizeValue.Length; i++)
        {
            ParticleSystem.MainModule mainModule = _particlseStartSizeValue[i].main;
            _initialValuesScale[i] = mainModule.startSize.Evaluate(1.0f);
        }

        for (int i = 0; i < _particlseStartSizeRange.Length; i++)
        {
            ParticleSystem.MainModule mainModule = _particlseStartSizeValue[i].main;
            float minValue = mainModule.startSize.constantMin;
            float maxValue = mainModule.startSize.constantMax;
            _initialRangesScale[i] = new Vector2(minValue, maxValue);
        }
    }

    private IEnumerator ExplosionRoutine(Vector3 targetExplosionScale, ParticleSystem[] particleArray1, ParticleSystem[] particleArray2)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _explosionDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / _explosionDuration;

            if (t < _explosionSlowDownPoint)
            {
                float fastGrowthFactor = Mathf.Pow(t / _explosionSlowDownPoint, 0.5f);

                for (int i = 0; i < particleArray1.Length; i++)
                {
                    ParticleSystem.MainModule mainModule = particleArray1[i].main;
                    mainModule.startSize = Mathf.Lerp(0f, targetExplosionScale.magnitude * _explosionSlowDownPoint, fastGrowthFactor);
                }

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
                float slowDownFactor = Mathf.SmoothStep(_explosionSlowDownPoint, 1f, (t - _explosionSlowDownPoint) / (1f - _explosionSlowDownPoint));

                for (int i = 0; i < particleArray1.Length; i++)
                {
                    ParticleSystem.MainModule mainModule = particleArray1[i].main;
                    mainModule.startSize = Mathf.Lerp(targetExplosionScale.magnitude * _explosionSlowDownPoint, targetExplosionScale.magnitude, slowDownFactor);
                }

                for (int i = 0; i < particleArray2.Length; i++)
                {
                    ParticleSystem.MainModule mainModule = particleArray2[i].main;
                    float minCurveValue = Mathf.Lerp(targetExplosionScale.magnitude * _explosionSlowDownPoint, targetExplosionScale.magnitude, slowDownFactor);
                    float maxCurveValue = Mathf.Lerp(targetExplosionScale.magnitude * _explosionSlowDownPoint * 1.5f, targetExplosionScale.magnitude * 1.5f, slowDownFactor);
                    mainModule.startSize = new ParticleSystem.MinMaxCurve(minCurveValue, maxCurveValue);
                }
            }

            yield return null;
        }

        for (int i = 0; i < particleArray1.Length; i++)
        {
            ParticleSystem.MainModule mainModule = particleArray1[i].main;
            mainModule.startSize = targetExplosionScale.magnitude;
        }

        for (int i = 0; i < particleArray2.Length; i++)
        {
            ParticleSystem.MainModule mainModule = particleArray2[i].main;
            mainModule.startSize = new ParticleSystem.MinMaxCurve(targetExplosionScale.magnitude, targetExplosionScale.magnitude * 1.5f);
        }
    }

    /*private IEnumerator EndExplosionRoutine()
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
    }*/

    public void DoExplosion(Vector3 targetExplosionScale)
    {
        StartCoroutine(ExplosionRoutine(targetExplosionScale, _particlseStartSizeValue, _particlseStartSizeRange));
    }
    /*public Coroutine EndExplosion()
    {
        return StartCoroutine(EndExplosionRoutine());
    }*/
    public void ResetExplosion()
    {
        StopAllCoroutines();
        for (int i = 0; i < _particlseStartSizeValue.Length; i++)
        {
            ParticleSystem.MainModule mainModule = _particlseStartSizeValue[i].main;
            mainModule.startSize = _initialValuesScale[i];
        }

        for (int i = 0; i < _particlseStartSizeRange.Length; i++)
        {
            ParticleSystem.MainModule mainModule = _particlseStartSizeValue[i].main;
            mainModule.startSize = new ParticleSystem.MinMaxCurve(_initialRangesScale[i].x, _initialRangesScale[i].y);
        }
    }
}
