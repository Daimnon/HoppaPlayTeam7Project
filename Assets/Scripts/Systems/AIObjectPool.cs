using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIObjectPool : MonoBehaviour
{
    [Header("Pool Details")]
    [SerializeField] private Adventurer[] _adventurerPrefabs;
    [SerializeField] private int _initialPoolSize = 100;

    private List<Adventurer> _aiPool;

    [Header("Spawn Related")]
    [SerializeField] private Transform[] _spawnPointsTr;
    [SerializeField] private int _spawnAmount;
    [SerializeField] private float _spawnDelay, _respawnDelay;
    //[SerializeField] private AdventurerType[] _spawnTypes;

    private WaitForSeconds _waitForRespawn;
    private const string _resourceCustomerTag = "ResourceCustomer", _weaponCustomerTag = "WeaponCustomer";

    private void Awake()
    {
        _aiPool = new List<Adventurer>();
        _waitForRespawn = new WaitForSeconds(_respawnDelay);
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
        for (int i = 0; i < _adventurerPrefabs.Length; i++)
        {
            for (int j = 0; j < _initialPoolSize; j++)
            {
                Adventurer adventurer = Instantiate(_adventurerPrefabs[i], transform);
                adventurer.gameObject.SetActive(false);
                _aiPool.Add(adventurer);
            }
        }

        SpawnResourceAdventurer(AdventurerType.Potions);
    }

    public Adventurer GetAdventurerFromPool(int adventurerTypeIndex)
    {
        for (int i = 0; i < _aiPool.Count; i++)
        {
            if (_aiPool[i].Type != (AdventurerType)adventurerTypeIndex)
                continue;

            Adventurer adventurer = _aiPool[i];
            if (!adventurer.gameObject.activeSelf)
            {
                adventurer.gameObject.SetActive(true);
                adventurer.transform.parent = null;
                adventurer.IsAlive = true;
                return adventurer;
            }
        }

        // If no inactive money objects are available, create a new one
        Adventurer newAdventurer = Instantiate(_adventurerPrefabs[adventurerTypeIndex], transform);
        _aiPool.Add(newAdventurer);
        newAdventurer.gameObject.SetActive(true);
        return newAdventurer;
    }
    public Adventurer GetAdventurerFromPool(Adventurer adventurer)
    {
        if (!_aiPool.Contains(adventurer))
        {
            Debug.Log("Adventurer not present is AI Pool");
            return null;
        }

        if (!adventurer.gameObject.activeSelf)
        {
            adventurer.gameObject.SetActive(true);
            adventurer.transform.parent = null;
            adventurer.IsAlive = true;
        }
        return adventurer;
    }

    public Adventurer SpawnAdventurer(Adventurer adventurer)
    {
        Adventurer respawnedAdventurer = GetAdventurerFromPool(adventurer);
        respawnedAdventurer.Agent.enabled = false;

        if (adventurer.CompareTag(_resourceCustomerTag))
            respawnedAdventurer.transform.SetParent(_spawnPointsTr[0]);
        else if (adventurer.CompareTag(_weaponCustomerTag))
            respawnedAdventurer.transform.SetParent(_spawnPointsTr[1]);

        respawnedAdventurer.transform.localPosition = Vector3.zero;
        respawnedAdventurer.transform.localRotation = Quaternion.identity;
        respawnedAdventurer.transform.parent = null;
        EventManager.InvokeAdventurerSpawned(respawnedAdventurer);
        return respawnedAdventurer;
    }
    
    public void ReturnAdventurerToPool(Adventurer adventurer)
    {
        adventurer.gameObject.SetActive(false);
        adventurer.transform.SetParent(transform);
        adventurer.transform.position = Vector3.zero;
    }

    public void RespawnAdventurer(Adventurer adventurer)
    {
        StartCoroutine(ResetAdventurer(adventurer));
    }
    private IEnumerator ResetAdventurer(Adventurer adventurer)
    {
        ReturnAdventurerToPool(adventurer);
        yield return _waitForRespawn;

        Adventurer respawnedAdventurer = GetAdventurerFromPool(adventurer);
        respawnedAdventurer.Agent.enabled = false;

        if (adventurer.CompareTag(_resourceCustomerTag))
            respawnedAdventurer.transform.SetParent(_spawnPointsTr[0]);
        else if (adventurer.CompareTag(_weaponCustomerTag))
            respawnedAdventurer.transform.SetParent(_spawnPointsTr[1]);

        respawnedAdventurer.transform.localPosition = Vector3.zero;
        respawnedAdventurer.transform.localRotation = Quaternion.identity;
        respawnedAdventurer.transform.parent = null;
        EventManager.InvokeAdventurerSpawned(respawnedAdventurer);
    }

    public void SpawnResourceAdventurer(AdventurerType adventurerType)
    {
        int amountSpawned = 0;
        for (int i = 0; i < _aiPool.Count; i++)
        {
            Adventurer adventurer = _aiPool[i];
            if (adventurer.CompareTag(_resourceCustomerTag) && adventurer.Type == adventurerType)
            {
                SpawnAdventurer(adventurer);
                amountSpawned++;

                if (amountSpawned >= _spawnAmount)
                    break;
            }
        }
    }
    public void SpawnWeaponAdventurer(AdventurerType adventurerType)
    {
        int amountSpawned = 0;
        for (int i = 0; i < _aiPool.Count; i++)
        {
            Adventurer adventurer = _aiPool[i];
            if (adventurer.CompareTag(_weaponCustomerTag) && adventurer.Type == adventurerType)
            {
                SpawnAdventurer(adventurer);
                amountSpawned++;

                if (amountSpawned >= _spawnAmount)
                    break;
            }
        }
    }
    private void OnLevelLaunched()
    {
        Initialize();
    }
}
