using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private AIObjectPool _aiPool;
    [SerializeField] private Transform _resourceTradingTr, _resourceExitTr;
    [SerializeField] private Transform _weaponTradingTr, _weaponExitTr;
    [SerializeField] private List<Adventurer> _adventurers;

    //[SerializeField] private Adventurer[] testingAdventurers;

    private const string _resourceCustomerTag = "ResourceCustomer", _weaponCustomer = "WeaponCustomer";

    private void Awake()
    {
        _adventurers = new List<Adventurer>();
    }
    private void OnEnable()
    {
        EventManager.OnWeaponTraderUnlocked += OnWeaponTraderUnlocked;
        EventManager.OnAdventurerSpawned += OnAdventurerSpawned;
    }
    private void OnDisable()
    {
        EventManager.OnWeaponTraderUnlocked -= OnWeaponTraderUnlocked;
        EventManager.OnAdventurerSpawned -= OnAdventurerSpawned;
    }

    private void OnWeaponTraderUnlocked(WeaponTrader weaponTrader)
    {
       _weaponTradingTr = weaponTrader.TradingPos;
        GameManager.Instance.AIPool.SpawnWeaponAdventurer(AdventurerType.AxeCommonSmall);
    }
    private void OnAdventurerSpawned(Adventurer adventurer)
    {
        if (adventurer.CompareTag(_resourceCustomerTag))
        {
            adventurer.TradingTr = _resourceTradingTr;
            adventurer.ExitTr = _resourceExitTr;
        }
        else if (adventurer.CompareTag(_weaponCustomer))
        {
            adventurer.TradingTr = _weaponTradingTr;
            adventurer.ExitTr = _weaponExitTr;
        }

        adventurer.StartRemotely();
        _adventurers.Add(adventurer);
    }
}
