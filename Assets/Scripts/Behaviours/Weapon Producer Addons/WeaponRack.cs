using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRack : WeaponProducerAddon
{
    [SerializeField] private Transform[] _smallProductsTr;
    public Transform[] SmallProductsTr => _smallProductsTr;

    [SerializeField] private Transform[] _mediumProductsTr;
    public Transform[] MediumProductsTr => _mediumProductsTr;

    [SerializeField] private Transform[] _largeProductsTr;
    public Transform[] LargeProductsTr => _largeProductsTr;

    private Transform[] _placements = null;
    public Transform[] Placements { get => _placements; set => _placements = value; }

    private List<Weapon> _weapons = null;
    public List<Weapon> Weapons { get => _weapons; set => _weapons = value; }

    private const string _playerTag = "Player";
    private PlayerInventory _playerInventory;

    private void Awake()
    {
        _weapons = new List<Weapon>();
        _placements = new Transform[0];
    }
    private void OnEnable()
    {
        EventManager.OnAnyAnvilUnlocked += OnAnyAnvilUnlocked;
        //EventManager.OnAnvilUnlocked += OnAnvilUnlocked;
    }
    private void OnDisable()
    {
        EventManager.OnAnyAnvilUnlocked -= OnAnyAnvilUnlocked;
        //EventManager.OnAnvilUnlocked -= OnAnvilUnlocked;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_weapons != null || _weapons.Count < 1) && other.CompareTag(_playerTag))
        {
            _playerInventory = other.GetComponent<PlayerInventory>();
            Weapon weaponToGive = _weapons[^1];
            _playerInventory.TakeWeapon(weaponToGive);
            _weapons.Remove(weaponToGive);
        }
    }

    private void OnAnyAnvilUnlocked(WeaponProducer weaponProducer)
    {
        weaponProducer.WeaponsRack = this;
    }
    /*private void OnAnvilUnlocked(WeaponProducer weaponProducer)
    {
        if (weaponProducer is AxeProducer)
            (weaponProducer as AxeProducer).WeaponRack = this;
    }*/
}
