using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public enum ItemStatus 
{ 
    Locked, 
    Equipped, 
    Unequipped 
}

public class Customizationmanager : MonoBehaviour, ISaveable
{
    [SerializeField] private List<CustomizationItemBase> _customizationItems = new();
    [SerializeField] private Transform _customizationContentTr;
    [SerializeField] private Player_Inventory _inventory;

    private void OnEnable()
    {
        EventManager.OnPurchase += OnPurchase;
    }
    private void Start()
    {
        for (int i = 0; i < _customizationItems.Count; i++)
        {
            _customizationItems[i].Inventory = _inventory;
        }
    }
    private void OnDisable()
    {
        EventManager.OnPurchase -= OnPurchase;
    }

    public CustomizationItemBase GetItem(int index)
    {
        for (int i = 0; i < _customizationItems.Count; i++)
        {
            if (i == index)
                return _customizationItems[i];
        }

        Debug.LogError("Item at index " + index + " does not exist.");
        return null;
    }
    public CustomizationItemBase GetItem(string name)
    {
        for (int i = 0; i < _customizationItems.Count; i++)
        {
            if (_customizationItems[i].Name == name)
                return _customizationItems[i];
        }

        Debug.LogError("Item " + name + " does not exist.");
        return null;
    }
    public CustomizationItemCurrency GetCurrencyItem(int index)
    {
        for (int i = 0; i < _customizationItems.Count; i++)
        {
            if (_customizationItems[i] is CustomizationItemCurrency && i == index)
                return _customizationItems[i] as CustomizationItemCurrency;
        }

        Debug.LogError("Item at index " + index + " is not a currency item.");
        return null;
    }
    public CustomizationItemCurrency GetCurrencyItem(string name)
    {
        for (int i = 0; i < _customizationItems.Count; i++)
        {
            if (_customizationItems[i] is CustomizationItemCurrency && _customizationItems[i].Name == name)
                return _customizationItems[i] as CustomizationItemCurrency;
        }

        Debug.LogError("Item " + name + "is not a currency item.");
        return null;
    }
    public CustomizationItemAd GetAdItem(int index)
    {
        for (int i = 0; i < _customizationItems.Count; i++)
        {
            if (_customizationItems[i] is CustomizationItemAd && i == index)
                return _customizationItems[i] as CustomizationItemAd;
        }

        Debug.LogError("Item at index " + index + " is not an ad item.");
        return null;
    }
    public CustomizationItemAd GetAdItem(string name)
    {
        for (int i = 0; i < _customizationItems.Count; i++)
        {
            if (_customizationItems[i] is CustomizationItemAd && _customizationItems[i].Name == name)
                return _customizationItems[i] as CustomizationItemAd;
        }

        Debug.LogError("Item " + name + "is not an ad item.");
        return null;
    }

    private void OnPurchase(string name)
    {
        EventManager.InvokeEquip(name);
    }

    public void LoadData(GameData gameData)
    {
        for (int i = 0; i < _customizationItems.Count; i++)
        {
            bool isPurchased;
            gameData.CustomizationItems.TryGetValue(_customizationItems[i].Name, out isPurchased);
            _customizationItems[i].IsPurchased = isPurchased;
            _customizationItems[i].transform.SetParent(_customizationContentTr);

            if (_customizationItems[i].Name == gameData.CurrentlyEquippedItem)
                _customizationItems[i].Equip();

            Debug.Log($"{i}: {_customizationItems[i].name}, {_customizationItems[i].IsPurchased}");
        }
    }
    public void SaveData(ref GameData gameData)
    {
        for (int i = 0; i < _customizationItems.Count; i++)
        {
            if (gameData.CustomizationItems.ContainsKey(_customizationItems[i].Name))
            {
                gameData.CustomizationItems.Remove(_customizationItems[i].Name);
            }
            gameData.CustomizationItems.Add(_customizationItems[i].Name, _customizationItems[i].IsPurchased);

            if (_customizationItems[i].Status == ItemStatus.Equipped)
                gameData.CurrentlyEquippedItem = _customizationItems[i].Name;
        }
    }
}
