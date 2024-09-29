using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    [SerializeField] float _timeToDieOut = 1.0f;
    private IEnumerator GrowFlameRoutine(Vector3 targetScale)
    {
        transform.localScale = Vector3.zero;

        float elapsedTime = 0f;
        Vector3 startingScale = Vector3.zero;

        while (elapsedTime < _timeToDieOut)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (_timeToDieOut);

            float growFactor = Mathf.SmoothStep(0f, 1f, t);
            transform.localScale = Vector3.Lerp(startingScale, targetScale, growFactor);

            yield return null;
        }
        transform.localScale = targetScale;
    }

    public IEnumerator ShrinkFlameRoutine()
    {
        float elapsedTime = 0f;
        Vector3 startingScale = transform.localScale;

        while (elapsedTime < _timeToDieOut / 3)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (_timeToDieOut / 3);

            float growFactor = Mathf.SmoothStep(0f, 1f, t);
            transform.localScale = Vector3.Lerp(startingScale, Vector3.zero, growFactor);

            yield return null;
        }
        transform.localScale = Vector3.zero;
    }
    public void GrowFlame(Vector3 targetScale)
    {
        StartCoroutine(GrowFlameRoutine(targetScale));
    }
}
