using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTrader : Trader
{
    [SerializeField] private GameObject _currencyPrefab;
    public override GameObject CurrencyPrefab => _currencyPrefab;

    [SerializeField] private Transform[] _productsTr;
    public override Transform[] ProductsTr => _productsTr;

    [SerializeField] private Transform _tradingPos;
    public override Transform TradingPos => _tradingPos;

    [SerializeField] private int[] _conversionRates = new int[System.Enum.GetValues(typeof(ResourceType)).Length];

    private const string _resourceCustomer = "ResourceCustomer";

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(_resourceCustomer) && _adventurers.Count < 1)
        {
            Adventurer adventurer = other.GetComponent<Adventurer>();

            if (!adventurer.HasBoughtItem && _playerInventory)
            {
                Object boughtItem = adventurer.BuyItem(_playerInventory);

                if (boughtItem is not Resource)
                    return;

                SellResource((boughtItem as Resource).Type);
                _adventurers.Remove(adventurer);
                return;
            }
        }

        if (!other.CompareTag(_playerTag) || _adventurers.Count < 1)
            return;

        if (_playerInventory != null && _playerInventory.ResourceCount < 1)
            return;

        Resource convertedResource = _playerInventory.PayFirstResource();
        SellResource(convertedResource.Type);
        EventManager.InvokePayFirstResource(convertedResource);
    }
    private void SellResource(ResourceType resourceToConvert)
    {
        int conversionRate = _conversionRates[(int)resourceToConvert];
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
