using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ConsumableObjectPool : MonoBehaviour
{
    [SerializeField] private Consumable[] _prefabs;
    [SerializeField] private int _initialPoolSize = 0;

    private List<Consumable> _objectPool;

    private void Awake()
    {
        _objectPool = new List<Consumable>();
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
        if (_prefabs == null)
            return;

        for (int i = 0; i < _prefabs.Length; i++)
        {
            for (int j = 0; j < _initialPoolSize; j++)
            {
                Consumable consumable = Instantiate(_prefabs[i], transform);
                consumable.gameObject.SetActive(false);
                _objectPool.Add(consumable);
            }
        }
    }

    /*public Consumable GetResourceFromPool(int consumableType)
    {
        for (int i = 0; i < _objectPool.Count; i++)
        {
            if (_objectPool[i].Type != (ResourceType)consumableType)
                continue;

            Consumable consumable = _objectPool[i];
            if (!consumable.gameObject.activeSelf)
            {
                consumable.gameObject.SetActive(true);
                return consumable;
            }
        }

        // If no inactive consumable is available, create a new one
        Consumable newConsumable = Instantiate(_prefabs[consumableType], transform);
        _objectPool.Add(newConsumable);
        newConsumable.gameObject.SetActive(true);
        return newConsumable;
    }*/
    public void ReturnConsumableToPool(Consumable consumable)
    {
        consumable.gameObject.SetActive(false);
        consumable.transform.SetParent(transform);
        consumable.transform.position = Vector3.zero;
        _objectPool.Add(consumable);
    }

    private void OnLevelLaunched()
    {
        Initialize();
    }
}