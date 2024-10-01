using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CustomizationItemBase : MonoBehaviour
{
    [Header("Item base")]
    [SerializeField] protected Image _background;
    [SerializeField] protected TMP_Text _title;
    [SerializeField] protected Image _icon;
    [SerializeField] protected Button _priceButton;
    [SerializeField] protected Player_Inventory _playerInventory;

    [Header("Text Color")]
    [SerializeField] protected Color _affordableColor = Color.white;
    [SerializeField] protected Color _expensiveColor = Color.red;

    [Header("Item Currency")]
    [SerializeField] protected int _price;
    [SerializeField] protected TMP_Text _priceText;

    public abstract void Buy();

     protected virtual void Awake()
    {
        UpdatePriceColor();
        EventManager.OnCurrencyChange += HandleCurrencyChanged;
    }

    protected virtual void OnDestroy()
    {
        EventManager.OnCurrencyChange -= HandleCurrencyChanged;
    }

    protected void UpdatePriceColor()
    {
        if (_playerInventory != null && _priceText != null) 
        {
            if (_playerInventory.Currency >= _price)
                _priceText.color = _affordableColor;
            else
                _priceText.color = _expensiveColor;
        }
        else
        {
            Debug.LogWarning("_playerInventory or _priceText is not assigned in the inspector.");
        }
    }

    private void HandleCurrencyChanged(int newCurrency)
    {
        UpdatePriceColor();
    }
}
