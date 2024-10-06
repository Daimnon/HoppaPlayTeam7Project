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
    private enum UpgradePriceMethod
    {
        Plus100,
        Times2
    }

    [Header("Components")]
    [SerializeField] private Player_Inventory _playerInventory;
    [SerializeField] private SoundManager _soundManager;

    [Header("Text Color")]
    [SerializeField] private Color _affordableColor = new(218, 196, 43, 255);
    [SerializeField] private Color _expensiveColor = Color.red;

    [Header("General")]
    [SerializeField] private string _maxLevelText = "Max!";

    [Header("Growth")]
    [SerializeField] private TextMeshProUGUI _growUpgradePriceText;
    [SerializeField] private int _growMaxLevel = 9;
    [SerializeField] private int _growthInitialCost = 500;
    private int _growUpgradeLevel = 0;

    [Header("Time")]
    [SerializeField] private TextMeshProUGUI _timeUpgradePriceText;
    [SerializeField] private int _timeMaxLevel = 30;
    [SerializeField] private int _timeInitialCost = 100;
    private int _timeUpgradeLevel = 0;

    [Header("Fire power")]
    [SerializeField] private TextMeshProUGUI _firePowerUpgradePriceText;
    [SerializeField] private int _firePowerMaxLevel = 7;
    [SerializeField] private int _firePowerInitialCost = 300;
    private int _firePowerUpgradeLevel = 0;


    private void Start()
    {
        if (!_playerInventory)
            _playerInventory = GetComponent<Player_Inventory>();

        if (!_soundManager)
            _soundManager = FindObjectOfType<SoundManager>();

        UpdatePriceUI();
    }

    private int GetUpgradeCost(int initialCost, int level, UpgradePriceMethod priceMethod)
    {
        if (priceMethod == UpgradePriceMethod.Times2)
            return initialCost * (int)Mathf.Pow(2, level);
        else if (priceMethod == UpgradePriceMethod.Plus100)
            return initialCost * (level +1);
        else
            return initialCost * (level +1);
    }
    /*private int GetUpgradeCost(int level)
    {
        return _initialCost * (level + 1);
    }
    private int GetGrowUpgradeCost()
    {
        return _initialCost * (int)Mathf.Pow(2, _growUpgradeLevel);
    }*/
    private void UpdatePriceText(TextMeshProUGUI priceText, int cost)
    {
        priceText.text = cost.ToString();

        if (_playerInventory.Currency >= cost)
            priceText.color = _affordableColor;
        else
            priceText.color = _expensiveColor;
    }
    private void UpdatePriceUI()
    {
        UpdatePriceText(_growUpgradePriceText, GetUpgradeCost(_growthInitialCost, _growUpgradeLevel, UpgradePriceMethod.Times2));
        UpdatePriceText(_timeUpgradePriceText, GetUpgradeCost(_timeInitialCost, _timeUpgradeLevel, UpgradePriceMethod.Plus100));
        UpdatePriceText(_firePowerUpgradePriceText, GetUpgradeCost(_firePowerInitialCost, _firePowerUpgradeLevel, UpgradePriceMethod.Times2));
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
        if (_growUpgradeLevel >= _growMaxLevel)
        {
            _growUpgradePriceText.text = _maxLevelText;
            _growUpgradePriceText.color = Color.white;
            return;
        }

        int cost = GetUpgradeCost(_growthInitialCost, _growUpgradeLevel, UpgradePriceMethod.Times2);
        int uiCost = GetUpgradeCost(_growthInitialCost, _growUpgradeLevel +1, UpgradePriceMethod.Times2);

        if (_playerInventory.Currency >= cost)
        {
            EventManager.InvokePayCurrency(cost);
            EventManager.InvokeUpgrade(UpgradeType.Grow);
            _growUpgradeLevel++;

            UpdatePriceText(_growUpgradePriceText, uiCost);
        }
    }
    public void UpgradeTime()
    {
        if (_timeUpgradeLevel >= _timeMaxLevel)
        {
            _timeUpgradePriceText.text = _maxLevelText;
            _timeUpgradePriceText.color = Color.white;
            return;
        }

        int cost = GetUpgradeCost(_timeInitialCost, _timeUpgradeLevel, UpgradePriceMethod.Plus100);
        int uiCost = GetUpgradeCost(_timeInitialCost, _timeUpgradeLevel + 1, UpgradePriceMethod.Plus100);

        if (_playerInventory.Currency >= cost)
        {
            EventManager.InvokePayCurrency(cost);
            EventManager.InvokeUpgrade(UpgradeType.Time);
            _timeUpgradeLevel++;

            UpdatePriceText(_timeUpgradePriceText, uiCost);
        }
    }
    public void UpgradeFirePower()
    {
        if (_firePowerUpgradeLevel >= _firePowerMaxLevel)
        {
            _firePowerUpgradePriceText.text = _maxLevelText;
            _firePowerUpgradePriceText.color = Color.white;
            return;
        }

        int cost = GetUpgradeCost(_firePowerInitialCost, _firePowerUpgradeLevel, UpgradePriceMethod.Times2);
        int uiCost = GetUpgradeCost(_firePowerInitialCost, _firePowerUpgradeLevel +1, UpgradePriceMethod.Times2);
        if (_playerInventory.Currency >= cost)
        {
            EventManager.InvokePayCurrency(cost);
            EventManager.InvokeUpgrade(UpgradeType.FirePower);
            _firePowerUpgradeLevel++;

            UpdatePriceText(_firePowerUpgradePriceText, uiCost);
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