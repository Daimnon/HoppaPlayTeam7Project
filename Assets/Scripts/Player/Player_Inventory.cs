using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{
    [SerializeField] private Player_HUD _hud;
    [SerializeField] private int _currency = 0;
    public int Currency => _currency;

    private void OnEnable()
    {
        EventManager.OnEarnCurrency += OnEarnCurrency;
    }
    private void OnDisable()
    {
        EventManager.OnEarnCurrency -= OnEarnCurrency;
    }

    private void OnEarnCurrency(int amount)
    {
        _currency += amount;
        _hud.UpdateCurrency(_currency);
    }
}
