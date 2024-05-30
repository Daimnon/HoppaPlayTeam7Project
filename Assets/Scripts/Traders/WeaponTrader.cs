using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrader : Trader
{
    [SerializeField] private GameObject _currencyPrefab;
    public override GameObject CurrencyPrefab => _currencyPrefab;

    [SerializeField] private Transform[] _productsTr;
    public override Transform[] ProductsTr => _productsTr;

    [SerializeField] private Transform _tradingPos;
    public override Transform TradingPos => _tradingPos;

    [SerializeField] private int[] _typePriceFactor = new int[3], _rarityPriceFactor = new int[3], _sizePriceFactor = new int[3];

    private void Start()
    {
        EventManager.InvokeWeaponTraderUnlocked(this);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(_playerTag) && _playerInventory.Weapon[0] && _adventurers.Count > 0)
        {
            SellWeapon(_playerInventory.GiveWeapon());
            _adventurers.RemoveAt(0);
            // move weapon to ai
        }
    }
    private void SellWeapon(Weapon weaponToSell)
    {
        int conversionRate = _typePriceFactor[(int)weaponToSell.Type] + _rarityPriceFactor[(int)weaponToSell.Rarity] + _sizePriceFactor[(int)weaponToSell.Size];
        int availableIndex = 0;
        int amountConverted = 0;

        while (amountConverted <= conversionRate && availableIndex < _productsTr.Length)
        {
            if (_productsTr[availableIndex].childCount > 0)
            {
                availableIndex++;
                continue;
            }

            Transform newCurrencyTr = _currencyObjectPool.GetCurrencyFromPool().transform;
            newCurrencyTr.SetParent(_productsTr[availableIndex]);
            newCurrencyTr.localPosition = Vector3.zero;
            amountConverted++;
            availableIndex++;
        }
    }
}
