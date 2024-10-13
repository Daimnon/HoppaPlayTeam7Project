using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CustomizationItemBase : MonoBehaviour
{
    [Header("Item base")]
    [SerializeField] protected Image _background;
    [SerializeField] protected TextMeshProUGUI _title;
    [SerializeField] protected Image _icon;
    [SerializeField] protected Button _priceButton, _equipBtn, _unequipBtn;
    [SerializeField] protected TextMeshProUGUI _priceText;
    [SerializeField] protected GameObject _itemOnPlayer;

    protected ItemStatus _status = ItemStatus.Locked;
    public ItemStatus Status { get => _status; set => _status = value; }

    protected Player_Inventory _inventory;
    public Player_Inventory Inventory { get => _inventory; set => _inventory = value; }

    [Header("Item Data")]
    [SerializeField] protected string _name;
    public string Name => _name;

    [SerializeField] protected int _price;
    [SerializeField] protected Color _affordableColor = Color.white;
    [SerializeField] protected Color _expensiveColor = Color.red;
    protected string _priceString;

    protected bool _isPurchased;
    public bool IsPurchased { get => _isPurchased; set => _isPurchased = value; }

    protected virtual void OnEnable()
    {
        EventManager.OnCurrencyChange += HandleCurrencyChanged;
        EventManager.OnEquip += OnEquip;

        UpdatePriceColor();
    }
    protected virtual void Start()
    {
        _title.text = _name;
        UpdatePriceColor();
    }
    protected virtual void OnDisable()
    {
        EventManager.OnCurrencyChange -= HandleCurrencyChanged;
        EventManager.OnEquip -= OnEquip;
    }

    protected void UpdatePriceColor()
    {
        if (_inventory != null && _priceText != null) 
        {
            if (_inventory.Currency >= _price)
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

    public void Unequip()
    {
        _status = ItemStatus.Unequipped;
        _priceButton.gameObject.SetActive(false);
        _unequipBtn.gameObject.SetActive(false);
        _equipBtn.gameObject.SetActive(true);
        _itemOnPlayer.SetActive(false);
    }
    public void Equip()
    {
        EventManager.InvokeEquip(_name);
    }
    public void ApplyStatus()
    {
        switch (_status)
        {
            case ItemStatus.Locked:
                _equipBtn.gameObject.SetActive(false);
                _unequipBtn.gameObject.SetActive(false);
                break;
            case ItemStatus.Equipped:
                Equip();
                break;
            case ItemStatus.Unequipped:
                Unequip();
                break;
        }
    }

    public abstract void Buy();

    private void OnEquip(string name)
    {
        if (name == _name)
        {
            _status = ItemStatus.Equipped;
            _priceButton.gameObject.SetActive(false);
            _equipBtn.gameObject.SetActive(false);
            _unequipBtn.gameObject.SetActive(true);
            _itemOnPlayer.SetActive(true);
        }
        else if (_status == ItemStatus.Locked)
        {
            return;
        }
        else
        {
            Unequip();
        }
    }
}
