using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ResourceObjectPool : MonoBehaviour
{
    [SerializeField] private Resource[] _resourcePrefabs;
    [SerializeField] private int _initialPoolSize = 200;

    private List<Resource> _resourcePool;

    private void Awake()
    {
        _resourcePool = new List<Resource>();
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
        for (int i = 0; i < _resourcePrefabs.Length; i++)
        {
            for (int j = 0; j < _initialPoolSize; j++)
            {
                Resource resource = Instantiate(_resourcePrefabs[i], transform);
                resource.gameObject.SetActive(false);
                _resourcePool.Add(resource);
            }
        }
    }
    
    public Resource GetResourceFromPool(int resourceType)
    {
        for (int i = 0; i < _resourcePool.Count; i++)
        {
            if (_resourcePool[i].Type != (ResourceType)resourceType)
                continue;

            Resource resource = _resourcePool[i];
            if (!resource.gameObject.activeSelf)
            {
                resource.gameObject.SetActive(true);
                return resource;
            }
        }

        // If no inactive money objects are available, create a new one
        Resource newResource = Instantiate(_resourcePrefabs[resourceType], transform);
        _resourcePool.Add(newResource);
        newResource.gameObject.SetActive(true);
        return newResource;
    }
    public void ReturnResourceToPool(Resource resource)
    {
        resource.gameObject.SetActive(false);
        resource.transform.SetParent(transform);
        resource.transform.position = Vector3.zero;
    }

    private void OnLevelLaunched()
    {
        Initialize();
    }
}