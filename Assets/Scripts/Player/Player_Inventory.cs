using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    [SerializeField] private int _currency = 0;
    public int Currency => _currency;

    [SerializeField] private int _specialCurrency = 0;
    public int SpecialCurrency => _specialCurrency;

    private void OnEnable()
    {
        EventManager.OnEarnCurrency += OnEarnCurrency;
        EventManager.OnPayCurrency += OnPayCurrency;
        EventManager.OnEarnSpecialCurrency += OnEarnSpecialCurrency;
        EventManager.OnPaySpecialCurrency += OnPaySpecialCurrency;
    }
    private void OnDisable()
    {
        EventManager.OnEarnCurrency -= OnEarnCurrency;
        EventManager.OnPayCurrency -= OnPayCurrency;
        EventManager.OnEarnSpecialCurrency -= OnEarnSpecialCurrency;
        EventManager.OnPaySpecialCurrency -= OnPaySpecialCurrency;
    }

    private void OnEarnCurrency(int amount)
    {
        _currency += amount;
        EventManager.InvokeCurrencyChange(_currency);
    }
    private void OnPayCurrency(int amount)
    {
        if (_currency - amount < 0)
        {
            // do can't pay pop up
            return;
        }

        _currency -= amount;
        EventManager.InvokeCurrencyChange(_currency);
    }

    private void OnEarnSpecialCurrency(int amount)
    {
        _specialCurrency += amount;
        EventManager.InvokeSpecialCurrencyChange(_specialCurrency);
    }
    private void OnPaySpecialCurrency(int amount)
    {
        if (_specialCurrency - amount < 0)
        {
            // do can't pay pop up
            return;
        }

        _specialCurrency -= amount;
        EventManager.InvokeSpecialCurrencyChange(_specialCurrency);
    }
}
