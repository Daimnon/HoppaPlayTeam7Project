using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
    [SerializeField] private int _worth = 1;
    [SerializeField] private Renderer _renderer;

    private const string _playerTag = "Player";
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_playerTag))
            return;

        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
        Pickup(playerInventory);
    }

    private void Pickup(PlayerInventory playerInventory)
    {
        playerInventory.EarnCurrency(_worth);
        playerInventory.CurrencyObjectPool.ReturnCurrencyToPool(this);
    }
}
