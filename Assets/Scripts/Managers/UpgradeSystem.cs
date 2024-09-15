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

    private int _growUpgradeLevel = 0;
    private int _timeUpgradeLevel = 0;
    private int _firePowerUpgradeLevel = 0;

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
        return 100 * (level + 1);
    }
    private void UpdatePriceText(TextMeshProUGUI priceText, int upgradeLevel)
    {
        int cost = GetUpgradeCost(upgradeLevel);
        priceText.text = cost.ToString();

        if (_playerInventory.Currency >= cost)
            priceText.color = _affordableColor;
        else
            priceText.color = _expensiveColor;
    }
    private void UpdatePriceUI()
    {
        UpdatePriceText(_timeUpgradePriceText, _timeUpgradeLevel);
        UpdatePriceText(_growUpgradePriceText, _growUpgradeLevel);
        UpdatePriceText(_firePowerUpgradePriceText, _firePowerUpgradeLevel);
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
    }

    public void UpgradeGrowth()
    {
        int cost = GetUpgradeCost(_growUpgradeLevel);
        if (_playerInventory.Currency >= cost)
        {
            EventManager.InvokePayCurrency(cost);
            EventManager.InvokeUpgrade(UpgradeType.Grow);
            _growUpgradeLevel++;

            _soundManager.PlayCoinSound();
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

            _soundManager.PlayCoinSound();
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
            
            _soundManager.PlayCoinSound();
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
