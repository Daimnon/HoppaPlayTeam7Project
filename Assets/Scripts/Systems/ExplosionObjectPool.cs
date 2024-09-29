using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionObjectPool : MonoBehaviour
{
    [SerializeField] private ParticleExplosion _prefab;
    [SerializeField] private int _initialPoolSize = 100;

    private List<ParticleExplosion> _objectPool;

    private void Awake()
    {
        _objectPool = new List<ParticleExplosion>();
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
            ParticleExplosion particleExplosion = Instantiate(_prefab, transform);
            particleExplosion.gameObject.SetActive(false);
            _objectPool.Add(particleExplosion);
        }
    }

    public ParticleExplosion GetParticleExplosionFromPool()
    {
        for (int i = 0; i < _objectPool.Count; i++)
        {
            ParticleExplosion particleExplosion = _objectPool[i];
            if (!particleExplosion.gameObject.activeSelf)
            {
                particleExplosion.gameObject.SetActive(true);
                return particleExplosion;
            }
        }

        // If no inactive falme is available, create a new one
        ParticleExplosion newParticleExplosion = Instantiate(_prefab, transform);
        _objectPool.Add(newParticleExplosion);
        newParticleExplosion.gameObject.SetActive(true);
        return newParticleExplosion;
    }
    public ParticleExplosion GetParticleExplosionFromPool(Vector3 targetPos)
    {
        for (int i = 0; i < _objectPool.Count; i++)
        {
            ParticleExplosion particleExplosion = _objectPool[i];
            if (!particleExplosion.gameObject.activeSelf)
            {
                targetPos.y += 0.1f;
                particleExplosion.transform.SetParent(null);
                particleExplosion.transform.position = targetPos;
                particleExplosion.gameObject.SetActive(true);
                return particleExplosion;
            }
        }

        // If no inactive falme is available, create a new one
        ParticleExplosion newParticleExplosion = Instantiate(_prefab, transform);
        _objectPool.Add(newParticleExplosion);
        newParticleExplosion.gameObject.SetActive(true);
        return newParticleExplosion;
    }
    public void ReturnParticleExplosionToPool(ParticleExplosion ParticleExplosion)
    {
        ParticleExplosion.gameObject.SetActive(false);
        ParticleExplosion.transform.SetParent(transform);
        ParticleExplosion.transform.position = Vector3.zero;
        _objectPool.Add(ParticleExplosion);
    }

    private void OnLevelLaunched()
    {
        Initialize();
    }
}
