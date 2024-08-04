using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    private int timerUpgradeLevel = 0;
    private int sizeUpgradeLevel = 0;
    private int firePowerUpgradeLevel = 0;

    private LevelManager levelManager;
    private Player_Controller playerController;
    private Player_Inventory playerInventory;
    private TerritoryClaimer territoryClaimer;
    private Player_HUD playerHUD;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        playerController = FindObjectOfType<Player_Controller>();
        playerInventory = FindObjectOfType<Player_Inventory>();
        territoryClaimer = FindObjectOfType<TerritoryClaimer>();
        playerHUD = FindObjectOfType<Player_HUD>();

        UpdateGoldUI();
    }

    void UpdateGoldUI()
    {
        playerHUD.UpdateCurrency(playerInventory.Currency);
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
        }
    }

    public void PurchaseSizeUpgrade()
    {
        int cost = GetUpgradeCost(sizeUpgradeLevel);
        if (playerInventory.Currency >= cost)
        {
            playerInventory.OnPayCurrency(cost);
            sizeUpgradeLevel++;
            playerController.IncreaseSize(sizeUpgradeLevel * 0.5f); // Example increment
            UpdateGoldUI();
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
        }
    }

    int GetUpgradeCost(int level)
    {
        return 100 * (level + 1);
    }
}
