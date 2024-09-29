using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FlameObjectPool : MonoBehaviour
{
    [SerializeField] private Flame _prefab;
    [SerializeField] private int _initialPoolSize = 100;

    private List<Flame> _objectPool;

    private void Awake()
    {
        _objectPool = new List<Flame>();
    }
    private void OnEnable()
    {
        EventManager.OnLevelLaunched += OnLevelLaunched;
    }
    private void OnDisable()
    {
        EventManager.OnLevelLaunched -= OnLevelLaunched;
    }

    private void Initialize()
    {
        if (_prefab == null)
            return;

        for (int i = 0; i < _initialPoolSize; i++)
        {
            Flame flame = Instantiate(_prefab, transform);
            flame.gameObject.SetActive(false);
            _objectPool.Add(flame);
        }
    }

    public Flame GetFlameFromPool()
    {
        for (int i = 0; i < _objectPool.Count; i++)
        {
            Flame flame = _objectPool[i];
            if (!flame.gameObject.activeSelf)
            {
                flame.gameObject.SetActive(true);
                return flame;
            }
        }

        // If no inactive falme is available, create a new one
        Flame newFlame = Instantiate(_prefab, transform);
        _objectPool.Add(newFlame);
        newFlame.gameObject.SetActive(true);
        return newFlame;
    }
    public Flame GetFlameFromPool(Vector3 targetPos)
    {
        for (int i = 0; i < _objectPool.Count; i++)
        {
            Flame flame = _objectPool[i];
            if (!flame.gameObject.activeSelf)
            {
                targetPos.y += 0.1f;
                flame.transform.SetParent(null);
                flame.transform.position = targetPos;
                flame.gameObject.SetActive(true);
                return flame;
            }
        }

        // If no inactive falme is available, create a new one
        Flame newFlame = Instantiate(_prefab, transform);
        _objectPool.Add(newFlame);
        newFlame.gameObject.SetActive(true);
        return newFlame;
    }
    public void ReturnFlameToPool(Flame Flame)
    {
        Flame.gameObject.SetActive(false);
        Flame.transform.SetParent(transform);
        Flame.transform.position = Vector3.zero;
        Flame.transform.localScale = Vector3.one;
        _objectPool.Add(Flame);
    }

    private void OnLevelLaunched()
    {
        Initialize();
    }
}