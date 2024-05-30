using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Unlocker : MonoBehaviour
{
    [Header("Details")]
    [SerializeField] protected GameObject _unlockablePrefab;
    [SerializeField] protected Transform _spawnPos;
    [SerializeField] private TextMeshProUGUI _currentPriceTxt;
    //[SerializeField] private int _priceToUnlock = 3, _amountToCharge = 1;

    //private PlayerInventory _playerInventory;
    private const string _playerTag = "Player";
    private bool _isUnlocked = false;

    private void OnTriggerEnter(Collider other)
    {
        /*if (_playerInventory == null && other.CompareTag(_playerTag))
            _playerInventory = other.GetComponent<PlayerInventory>();*/
    }
    private void OnTriggerStay(Collider other)
    {
        if (_isUnlocked)
            return;

        /*int paidAmount = _playerInventory.PayCurrency(_amountToCharge);
        EventManager.InvokePayCurrency(paidAmount);
        _priceToUnlock -= paidAmount;
        _currentPriceTxt.text = _priceToUnlock.ToString();

        if (!_isUnlocked && _priceToUnlock <= 0)
        {
            _isUnlocked = true;
            Unlock();
        }*/
    }
    protected void SpawnNewProp()
    {
        Instantiate(_unlockablePrefab, _spawnPos.position, _spawnPos.rotation);
        EventManager.InvokeBakeNavMesh();
        Destroy(gameObject);
    }
    protected virtual void Unlock()
    {
        EventManager.InvokeUnlock();
        SpawnNewProp();
    }
}
