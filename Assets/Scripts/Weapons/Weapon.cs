using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponType _type;
    public WeaponType Type => _type;

    [SerializeField] private WeaponRarity _rarity;
    public WeaponRarity Rarity => _rarity;

    [SerializeField] private WeaponSize _size;
    public WeaponSize Size => _size;

    [SerializeField] private Renderer _renderer;
    [SerializeField] private bool _isInInventory = false;

    private const string _playerTag = "Player";
    private void OnTriggerEnter(Collider other) // might conflict with producer's trigger - take notice.
    {
        if (!_isInInventory && other.CompareTag(_playerTag))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            Pickup(playerInventory);
        }
    }

    private void Pickup(PlayerInventory playerInventory)
    {
        //playerInventory.TakeWeapon(this);
        _isInInventory = true;
    }
}
