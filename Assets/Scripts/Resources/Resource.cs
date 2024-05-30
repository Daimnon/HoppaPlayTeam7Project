using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private ResourceType _type;
    public ResourceType Type => _type;

    [SerializeField] private Renderer _renderer;

    [SerializeField] private bool _isInInventory = false;
    public bool IsInInventory { get => _isInInventory; set => _isInInventory = value; }

    private List<Resource> _originTrList = null;
    public List<Resource> OriginTrList { get => _originTrList; set => _originTrList = value; }

    private const string _playerTag = "Player";
    private void OnTriggerEnter(Collider other) // might conflict with producer's trigger - take notice.
    {
        if (!_isInInventory && other.CompareTag(_playerTag))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            Pickup(playerInventory);
        }
    }

    private void Pickup(PlayerInventory playerInventory)
    {
        if (_originTrList != null)
            _originTrList.Remove(this);

        playerInventory.TakeResource(this);
        _isInInventory = true;
    }
}