using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Collections;

public enum UpgradeType
{
    Grow,
    Time,
    FirePower
}

public class UpgradeSystem : MonoBehaviour, ISaveable
{
    [Header("Components")]
    [SerializeField] private Player_Inventory _playerInventory;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private TextMeshProUGUI _growUpgradePriceText, _timeUpgradePriceText, _firePowerUpgradePriceText;

    [Header("Text Color")]
    [SerializeField] private Color _affordableColor = new(218, 196, 43, 255);
    [SerializeField] private Color _expensiveColor = Color.red;

    [Header("Data")]
    [SerializeField] private int _growMaxLevel = 7;
    [SerializeField] private int _timeMaxLevel = 30;
    [SerializeField] private int _firePowerMaxLevel = 7;

    private int _growUpgradeLevel = 0;
    private int _timeUpgradeLevel = 0;
    private int _firePowerUpgradeLevel = 0;

    private const int _initialCost = 100;

    private void Start()
    {
        if (!_playerInventory)
            _playerInventory = GetComponent<Player_Inventory>();

        if (!_soundManager)
            _soundManager = FindObjectOfType<SoundManager>();

        UpdatePriceUI();
    }
    
    private int GetUpgradeCost(int level)
    {
        return _initialCost * (level + 1);
    }
    private int GetGrowUpgradeCost()
    {
        return _initialCost * (int)Mathf.Pow(2, _growUpgradeLevel);
    }
    private void UpdatePriceText(TextMeshProUGUI priceText, int upgradeLevel, int maxUpgradeLevel)
    {
        if (upgradeLevel >= maxUpgradeLevel)
        {
            priceText.text = "Max!";
            priceText.color = Color.white;
            return;
        }

        int cost = GetUpgradeCost(upgradeLevel);
        priceText.text = cost.ToString();

        if (_playerInventory.Currency >= cost)
            priceText.color = _affordableColor;
        else
            priceText.color = _expensiveColor;
    }
    private void UpdateGrowthPriceText(TextMeshProUGUI priceText)
    {
        if (_growUpgradeLevel >= _growMaxLevel)
        {
            _growUpgradePriceText.text = "Max!";
            _growUpgradePriceText.color = Color.white;
            return;
        }

        int cost = GetGrowUpgradeCost();
        _growUpgradePriceText.text = cost.ToString();

        if (_playerInventory.Currency >= cost)
            _growUpgradePriceText.color = _affordableColor;
        else
            _growUpgradePriceText.color = _expensiveColor;
    }
    private void UpdatePriceUI()
    {
        UpdateGrowthPriceText(_growUpgradePriceText);
        UpdatePriceText(_timeUpgradePriceText, _timeUpgradeLevel, _timeMaxLevel);
        UpdatePriceText(_firePowerUpgradePriceText, _firePowerUpgradeLevel, _firePowerMaxLevel);
    }

    private IEnumerator UpdatePricesAfterLoad(bool isNewLevel)
    {
        yield return null;
        UpdatePriceUI();

        if (isNewLevel) yield break;

        EventManager.InvokeMultipleGrowth(_growUpgradeLevel);

        for (int i = 0; i < _timeUpgradeLevel; i++)
        {
            EventManager.InvokeUpgrade(UpgradeType.Time);
        }

        for (int i = 0; i < _firePowerUpgradeLevel; i++)
        {
            EventManager.InvokeUpgrade(UpgradeType.FirePower);
        }
    }

    public void UpgradeGrowth()
    {
        if (_growUpgradeLevel >= _growMaxLevel) return;

        int cost = GetGrowUpgradeCost();
        if (_playerInventory.Currency >= cost)
        {
            EventManager.InvokePayCurrency(cost);
            EventManager.InvokeUpgrade(UpgradeType.Grow);
            _growUpgradeLevel++;

            UpdatePriceUI();
        }
    }
    public void UpgradeTime()
    {
        int cost = GetUpgradeCost(_timeUpgradeLevel);
        if (_playerInventory.Currency >= cost)
        {
            EventManager.InvokePayCurrency(cost);
            EventManager.InvokeUpgrade(UpgradeType.Time);
            _timeUpgradeLevel++;

            UpdatePriceUI();
        }
    }
    public void UpgradeFirePower()
    {
        int cost = GetUpgradeCost(_firePowerUpgradeLevel);
        if (_playerInventory.Currency >= cost)
        {
            EventManager.InvokePayCurrency(cost);
            EventManager.InvokeUpgrade(UpgradeType.FirePower);
            _firePowerUpgradeLevel++;
            
            UpdatePriceUI();
        }
    }

    public void LoadData(GameData gameData)
    {
        if (gameData.IsNewLevel)
        {
            gameData.IsNewLevel = false;
            StartCoroutine(UpdatePricesAfterLoad(true));
            return;
        }

        _growUpgradeLevel = gameData.GrowUpgradeLevel;
        _timeUpgradeLevel = gameData.TimeUpgradeLevel;
        _firePowerUpgradeLevel = gameData.FirePowerUpgradeLevel;
        StartCoroutine(UpdatePricesAfterLoad(false));
    }
    public void SaveData(ref GameData gameData)
    {
        gameData.GrowUpgradeLevel = _growUpgradeLevel;
        gameData.TimeUpgradeLevel = _timeUpgradeLevel;
        gameData.FirePowerUpgradeLevel = _firePowerUpgradeLevel;
    }
}