using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trader : MonoBehaviour
{
    public abstract GameObject CurrencyPrefab { get; }
    public abstract Transform[] ProductsTr { get; }
    public abstract Transform TradingPos { get; }

    protected CurrencyObjectPool _currencyObjectPool;
    protected PlayerInventory _playerInventory;
    protected CinemachineFramingTransposer _framingTransposer;
    protected List<Adventurer> _adventurers;
    protected const string _playerTag = "Player";
    protected const string _aiTag = "AI";
    
    private void Awake()
    {
        _adventurers = new List<Adventurer>();
    }
    private void Start()
    {
        _currencyObjectPool = GameManager.Instance.CurrencyPool;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_aiTag)) // do entering animation on self
        {
            _adventurers.Add(other.GetComponent<Adventurer>());
        }
        else if (other.CompareTag(_playerTag) && !_playerInventory)
        {
            _playerInventory = other.GetComponent<PlayerInventory>();
            GameManager.Instance.VirtualCam.Follow = TradingPos;
        }
        else
            return;
        // do entering animation on self
    }
    private void OnTriggerExit(Collider other)
    {
        /*if (other.CompareTag(_aiTag))
            _adventurers.Remove(other.GetComponent<Adventurer>()); // maybe should remove*/
        if (other.CompareTag(_playerTag)) // do exiting animation on self
        {
            GameManager.Instance.VirtualCam.Follow = _playerInventory.transform;
            _framingTransposer = null;
            _playerInventory = null;
        }
        else
            return;
        // do exiting animation on self
    }
}
