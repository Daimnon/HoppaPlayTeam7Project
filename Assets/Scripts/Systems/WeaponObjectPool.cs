using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObjectPool : MonoBehaviour
{
    [SerializeField] private Weapon[] _weaponPrefabs;
    [SerializeField] private int _initialPoolSize = 100;

    private List<Weapon> _weaponPool;

    private int _uniqueWeaponsCount = 0;
    public int UniqueWeaponsCount => _uniqueWeaponsCount;


    private void Awake()
    {
        _weaponPool = new List<Weapon>();
        _uniqueWeaponsCount = _weaponPrefabs.Length;
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
        for (int i = 0; i < _weaponPrefabs.Length; i++)
        {
            for (int j = 0; j < _initialPoolSize; j++)
            {
                Weapon weapon = Instantiate(_weaponPrefabs[i], transform);
                weapon.gameObject.SetActive(false);
                _weaponPool.Add(weapon);
            }
        }
    }

    public Weapon GetWeaponFromPool(WeaponType type, WeaponRarity rarity, WeaponSize size)
    {
        for (int i = 0; i < _weaponPool.Count; i++)
        {
            Weapon weapon = _weaponPool[i];
            if (weapon.Type != type || weapon.Rarity != rarity || weapon.Size != size) // check all conditions
                continue;

            if (!weapon.gameObject.activeSelf)
            {
                weapon.gameObject.SetActive(true);
                return weapon;
            }
        }

        // If no inactive weapon copy are available, create a new one
        for (int i = 0; i < _weaponPrefabs.Length; i++)
        {
            Weapon weaponPrefab = _weaponPrefabs[i];
            if (weaponPrefab.Type != type || weaponPrefab.Rarity != rarity || weaponPrefab.Size != size) // check all conditions
                continue;

            Weapon newWeapon = Instantiate(weaponPrefab, transform);
            _weaponPool.Add(newWeapon);
            newWeapon.gameObject.SetActive(true);
            return newWeapon;
        }
        
        // if reaches here than something went wrong
        return null;
    }
    public void ReturnWeaponToPool(Weapon weapon)
    {
        weapon.gameObject.SetActive(false);
        weapon.transform.SetParent(transform);
        weapon.transform.position = Vector3.zero;
    }

    private void OnLevelLaunched()
    {
        Initialize();
    }
}
