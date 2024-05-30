using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : Character
{
    [Header("Currency Data")]
    [SerializeField] private CurrencyObjectPool _currencyObjectPool;
    public CurrencyObjectPool CurrencyObjectPool => _currencyObjectPool;

    [SerializeField] private int _currency = 0;
    public int Currency => _currency;

    [Header("Resources Data")]
    [SerializeField] private int _resourcesCarryLimit = 3;
    public int ResourcesCarryLimit => _resourcesCarryLimit;

    [SerializeField] private float _maxTextOffset = 0.5f;

    [SerializeField] private List<Transform> _resourcesTr;
    public List<Transform> ResourcesTr => _resourcesTr;

    [SerializeField] private ResourceObjectPool _resourceObjectPool;
    public ResourceObjectPool ResourceObjectPool => _resourceObjectPool;

    [SerializeField] private List<Resource> _resources = new(); // testing requires serialization
    public List<Resource> Resources => _resources;

    [SerializeField] private int _resourceCount = 0; // testing requires serialization
    public int ResourceCount => _resourceCount;

    [Header("Weapon Data")]
    [SerializeField] private int _weaponCarryLimit = 1;
    public int WeaponCarryLimit => _weaponCarryLimit;

    [SerializeField] private Transform[] _weaponsTr;
    public Transform[] WeaponsTr => _weaponsTr;

    [SerializeField] private WeaponObjectPool _weaponObjectPool;
    public WeaponObjectPool WeaponObjectPool => _weaponObjectPool;

    private List<Weapon> _weapon;
    public List<Weapon> Weapon => _weapon;

    [Header("UI")]
    [SerializeField] private RectTransform _canvasRTr;
    [SerializeField] private TextMeshProUGUI _currencyTxt, _resourceTxt;

    private void Awake()
    {
        _weapon = new List<Weapon>(1) { null };
    }
    private void Start()
    {
        bool isHoldingResource = _resources.Count > 0 ? true : false;
        EventManager.InvokeHoldResource(this, isHoldingResource);
    }

    private void ShowMaxResourcesText(bool shouldShow)
    {
        if (shouldShow)
        {
            _canvasRTr.anchoredPosition = new Vector2(0, _resources[_resourceCount - 1].transform.position.y + _maxTextOffset);
            _canvasRTr.gameObject.SetActive(true);
        }
        else
        {
            _canvasRTr.gameObject.SetActive(false);
        }
    }
    public void EarnCurrency(int addedCurrency)
    {
        _currency += addedCurrency;
        _currencyTxt.text = _currency.ToString();
    }
    public int PayCurrency(int price)
    {
        if (_currency <= 0)
        {
            Debug.Log("No more currency");
            return 0;
        }

        _currency -= price;
        _currencyTxt.text = _currency.ToString();
        return price;
    }

    public void TakeResource(Resource newResource)
    {
        if (_resources.Count >= _resourcesCarryLimit)
        {
            Debug.Log("Carry limit reached");
            return;
        }

        _resources.Add(newResource);
        _resourceCount++;

        if (_resourceCount == 1)
            EventManager.InvokeHoldResource(this, true);

        int lastResourceIndex = _resourceCount - 1;
        newResource.transform.SetParent(_resourcesTr[lastResourceIndex]);
        newResource.transform.localPosition = Vector3.zero;
        newResource.transform.localRotation = Quaternion.identity;
        newResource.IsInInventory = true;   

        if (_resources.Count == _resourcesCarryLimit)
        {
            _canvasRTr.anchoredPosition = new Vector2(0, _resources[lastResourceIndex].transform.position.y);
            _canvasRTr.gameObject.SetActive(true);
        }
    }
    public Resource PayFirstResource()
    {
        if (_resources.Count <= 0)
        {
            Debug.Log("No resource to give");
            return null;
        }

        Resource resourceToPay = _resources[_resources.Count - 1];
        _resources.Remove(resourceToPay);
        _resourceCount--;
        resourceToPay.IsInInventory = false;

        if (_resourceCount == 0)
            EventManager.InvokeHoldResource(this, false);

        _canvasRTr.gameObject.SetActive(false);
        return resourceToPay;
    }
    public Resource PayResource(ResourceType wantedReseource)
    {
        if (_resources.Count <= 0)
        {
            Debug.Log("No resource to give");
            return null;
        }

        Resource resourceToGive = null;
        for (int i = _resources.Count - 1; i >= 0; i--)
        {
            if (_resources[i].Type == wantedReseource)
            {
                resourceToGive = _resources[i];
                break;
            }

            if (i == 0 && _resources[i].Type != wantedReseource)
            {
                Debug.Log("Resource type not found.");
                return null;
            }
        }

        _resources.Remove(resourceToGive);
        _resourceCount--;
        resourceToGive.IsInInventory = false;

        if (_resourceCount == 0)
            EventManager.InvokeHoldResource(this, false);

        _canvasRTr.gameObject.SetActive(false);

        return resourceToGive;
    }
    public void TakeWeapon(Weapon newWeapon)
    {
        if (_weapon.Count < 1)
            return;

        if (!_weapon[^1])
            _weapon.Remove(_weapon[^1]);

        _weapon.Add(newWeapon);
        EventManager.InvokeHoldWeapon(this, true);

        int weaponTrIndex = (int)newWeapon.Size;
        newWeapon.transform.SetParent(_weaponsTr[weaponTrIndex]);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
    }
    public Weapon GiveWeapon()
    {
        Weapon currentWeapon = _weapon[0];
        _weapon.Remove(currentWeapon);
        //_canvasRTr.gameObject.SetActive(false);

        return currentWeapon;
    }
    public Weapon GiveWeapon(WeaponType type, WeaponRarity rarity, WeaponSize size)
    {
        Weapon currentWeapon = _weapon[0];
        if (!currentWeapon || currentWeapon.Type != type || currentWeapon.Rarity != rarity|| currentWeapon.Size != size)
        {
            Debug.Log("No resource to give");
            return null;
        }

        _weapon.Remove(currentWeapon);
        //_canvasRTr.gameObject.SetActive(false);

        return currentWeapon;
    }
}
