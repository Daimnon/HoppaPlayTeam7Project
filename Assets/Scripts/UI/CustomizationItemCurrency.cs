using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationItemCurrency : CustomizationItemBase
{
    protected override void Start()
    {
        base.Start();

        _priceString = _price.ToString();
        _priceText.text = _priceString;

        UpdatePriceColor();
    }

    public override void Buy()
    {
        if (_inventory.Currency >= _price)
        {
            EventManager.InvokePayCurrency(_price);
            UpdatePriceColor();
            _isPurchased = true;
            EventManager.InvokePurchase(_name);
            Debug.Log($"Purchased {_title.text} for {_price} coins.");
        }
        else
        {
            _priceText.color = _expensiveColor;
            Debug.Log("Not enough currency to purchase or _playerInventory is not assigned.");
        }
    }
}
