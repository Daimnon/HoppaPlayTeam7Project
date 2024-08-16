using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UpgradeSystem : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TMP_Text timerUpgradePriceText;
    [SerializeField] private TMP_Text sizeUpgradePriceText;
    [SerializeField] private TMP_Text firePowerUpgradePriceText;

    private int timerUpgradeLevel = 0;
    private int sizeUpgradeLevel = 0;
    private int firePowerUpgradeLevel = 0;

    private LevelManager levelManager;
    private Player_Controller playerController;
    private Player_Inventory playerInventory;
    private TerritoryClaimer territoryClaimer;
    private Player_HUD playerHUD;
    private Player_Data playerData;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        playerController = FindObjectOfType<Player_Controller>();
        playerInventory = FindObjectOfType<Player_Inventory>();
        territoryClaimer = FindObjectOfType<TerritoryClaimer>();
        playerHUD = FindObjectOfType<Player_HUD>();
        playerData = playerController.GetComponent<Player_Data>();

        UpdateGoldUI();
        UpdatePriceUI();
    }

    private void UpdateGoldUI()
    {
        playerHUD.UpdateCurrency(playerInventory.Currency);
    }

    private void UpdatePriceUI()
    {
        UpdatePriceText(timerUpgradePriceText, timerUpgradeLevel);
        UpdatePriceText(sizeUpgradePriceText, sizeUpgradeLevel);
        UpdatePriceText(firePowerUpgradePriceText, firePowerUpgradeLevel);
    }

    private void UpdatePriceText(TMP_Text priceText, int upgradeLevel)
    {
        int cost = GetUpgradeCost(upgradeLevel);
        priceText.text = cost.ToString();

        if (playerInventory.Currency >= cost)
        {
            priceText.color = Color.black;
        }
        else
        {
            priceText.color = Color.red;
        }
    }

    public void PurchaseTimerUpgrade()
    {
        int cost = GetUpgradeCost(timerUpgradeLevel);
        if (playerInventory.Currency >= cost)
        {
            playerInventory.OnPayCurrency(cost);
            timerUpgradeLevel++;
            levelManager.ExtendTime(2);
            UpdateGoldUI();
            UpdatePriceUI();
        }
    }

    public void PurchaseSizeUpgrade()
    {
        int cost = GetUpgradeCost(sizeUpgradeLevel);
        if (playerInventory.Currency >= cost)
        {
            playerInventory.OnPayCurrency(cost);
            sizeUpgradeLevel++;
            playerData.LevelUp(); // Level increment
            UpdateGoldUI();
            UpdatePriceUI();
        }
    }

    public void PurchaseFirePowerUpgrade()
    {
        int cost = GetUpgradeCost(firePowerUpgradeLevel);
        if (playerInventory.Currency >= cost)
        {
            playerInventory.OnPayCurrency(cost);
            firePowerUpgradeLevel++;
            territoryClaimer.IncreaseFirePower(firePowerUpgradeLevel * 1.0f, firePowerUpgradeLevel * 0.5f); // Example increments
            UpdateGoldUI();
            UpdatePriceUI();
        }
    }

    private int GetUpgradeCost(int level)
    {
        return 100 * (level + 1);
    }
}
