using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour, ISaveable
{
    [SerializeField] private int _currency = 0;
    public int Currency => _currency;

    [SerializeField] private int _specialCurrency = 0;
    public int SpecialCurrency => _specialCurrency;

    [Header("Rewards for pogression")]
    private int _progressionCurrency = 0;
    private int _objectiveBonus = 500;
    private float _timeBonusMultiplier = 100f;

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

    public void CalculateTimeBonus(float timeRemaining)
    {
        int bonus = Mathf.RoundToInt(timeRemaining * _timeBonusMultiplier);
        OnEarnCurrency(bonus);
        Debug.Log($"<color=red>timeRemaining: {timeRemaining}, time bonus(*100): {bonus} </color>");

    }

    public void CalculateProgressionReward(float progressionPercentage, int starsEarned)
    {
        _progressionCurrency = Mathf.RoundToInt(progressionPercentage * 30);
        int totalReward = _progressionCurrency + starsEarned * _objectiveBonus;
        Debug.Log($"<color=red>progressionPercentage: {progressionPercentage}, _progressionCurrency: {_progressionCurrency},\n starsEarned: {starsEarned},\n totalReward: {totalReward} </color>");
        OnEarnCurrency(totalReward);
    }

    public void LoadData(GameData gameData)
    {
        _currency = gameData.Currency;
        _specialCurrency = gameData.SpecialCurrency;
    }
    public void SaveData(ref GameData gameData)
    {
        gameData.Currency = _currency;
        gameData.SpecialCurrency = _specialCurrency;
    }
}
