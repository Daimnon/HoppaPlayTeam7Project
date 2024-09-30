using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationItemCurrency : CustomizationItemBase
{
    protected override void Awake()
    {
        base.Awake();

        if (_priceText != null)
        {
            _priceText.text = _price.ToString();
        }
        else
        {
            Debug.LogWarning("_priceText is not assigned in the inspector.");
        }

        UpdatePriceColor();
    }

    public override void Buy()
    {
        if (_playerInventory != null && _playerInventory.Currency >= _price)
        {
            EventManager.InvokePayCurrency(_price);
            UpdatePriceColor();
            Debug.Log($"Purchased {_title.text} for {_price} coins.");
        }
        else
        {
            if (_priceText != null)
            {
                _priceText.color = _expensiveColor;
            }
            Debug.Log("Not enough currency to purchase or _playerInventory is not assigned.");
        }
    }
}
