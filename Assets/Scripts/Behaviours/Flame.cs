using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    private Vector3 _initialScale;
    private Vector3 _targetScale = new Vector3(6.0f, 6.0f, 6.0f);
    public Vector3 TargetScale => _targetScale;  // The desired target size
    public float _duration = 2f; // Total time for the scaling effect
    public float _slowDownPoint = 0.8f; // Percentage where the scaling slows down


    private void Start()
    {
        _initialScale = transform.localScale;
    }
    public void DoExplosion()
    {
        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the percentage of completion
            float t = elapsedTime / _duration;

            if (t < _slowDownPoint)
            {
                // Fast growth phase (exponential or linear)
                float fastGrowthFactor = Mathf.Pow(t / _slowDownPoint, 0.5f); // Adjust the exponent for faster or slower growth
                transform.localScale = Vector3.Lerp(_initialScale, _targetScale * _slowDownPoint, fastGrowthFactor);
            }
            else
            {
                // Slow growth phase (ease out)
                float slowDownFactor = Mathf.SmoothStep(_slowDownPoint, 1f, (t - _slowDownPoint) / (1f - _slowDownPoint));
                transform.localScale = Vector3.Lerp(_initialScale * _slowDownPoint, _targetScale, slowDownFactor);
            }

            yield return null; // Wait for the next frame
        }

        // Ensure the object reaches the target scale exactly
        transform.localScale = _targetScale;
    }
}
