using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionProducer : ResourceProducer
{
    [Header("Product Source")]
    [SerializeField] private ResourceObjectPool _resourcePool;
    public override ResourceObjectPool ResourcePool { get => _resourcePool; set => _resourcePool = value; }

    [SerializeField] private MushroomToPotionConverter _inventory;
    public MushroomToPotionConverter Inventory => _inventory;

    [Header("Production Details")]
    [SerializeField] private ResourceProducerType _type = ResourceProducerType.AlchemyTable;
    public override ResourceProducerType Type => _type;

    [SerializeField] private int _upgradeFactor = 3;
    public override int UpgradeFactor { get => _upgradeFactor; }

    [SerializeField] private int _maxProducts = 0;
    public override int MaxProducts { get => _maxProducts; set => _maxProducts = value; }

    [SerializeField] private float _productionTime = 1.0f;
    public override float ProductionTime => _productionTime;

    [SerializeField] private bool _isFull = false;
    public override bool IsFull { get => _isFull; set => _isFull = value; }

    [Header("Product placements")]
    [SerializeField] private Transform[] _productsTr;
    public override Transform[] ProductsTr => _productsTr;

    private List<Resource> _products;
    public override List<Resource> Products => _products;

    private const string _playerTag = "Player";
    private PlayerInventory _playerInventory;

    private void Awake()
    {
        _products = new List<Resource>();
    }
    private void Start()
    {
        Initialize();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
            _playerInventory = other.GetComponent<PlayerInventory>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (!_playerInventory)
            return;

        Produce();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
            _playerInventory = null;
    }

    public override void Produce() // need to modify for fuel usage
    {
        if (_playerInventory.ResourceCount < _playerInventory.ResourcesCarryLimit)
        {
            if (!_inventory.TryUsePotion())
                return;

            int resourceIndex = (int)_type;
            Resource newResource = _resourcePool.GetResourceFromPool(resourceIndex);
            _playerInventory.TakeResource(newResource);
        }
    }
}
