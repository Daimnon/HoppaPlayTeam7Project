using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomProducer : ResourceProducer
{
    [Header("Product Source")]
    [SerializeField] private ResourceObjectPool _resourcePool;
    public override ResourceObjectPool ResourcePool { get => _resourcePool; set => _resourcePool = value; }

    [Header("Production Details")]
    [SerializeField] private ResourceProducerType _type = ResourceProducerType.MushroomCluster;
    public override ResourceProducerType Type => _type;

    [SerializeField] private int _upgradeFactor = 3;
    public override int UpgradeFactor { get => _upgradeFactor; }

    [SerializeField] private int _maxProducts = 3;
    public override int MaxProducts { get => _maxProducts; set => _maxProducts = value; }

    [SerializeField] private float _productionTime = 0.1f;
    public override float ProductionTime => _productionTime;

    [SerializeField] private bool _isFull = false;
    public override bool IsFull { get => _isFull; set => _isFull = value; }

    private bool _isProducing = false;

    [Header("Product placements")]
    [SerializeField] private Transform[] _productsTr;
    public override Transform[] ProductsTr => _productsTr;

    private List<Resource> _products;
    public override List<Resource> Products => _products;

    private void Awake()
    {
        _products = new List<Resource>();
    }
    private void Start()
    {
        Initialize();
        Produce();
    }

    public override void Produce()
    {
        if (_isFull)
            return;

        _isProducing = true;
        int productCount = _products.Count;
        Transform productTr = null;

        if (productCount < _maxProducts && _productsTr[productCount].childCount < 1)
        {
            int resourceIndex = (int)_type;
            Resource newResource = _resourcePool.GetResourceFromPool(resourceIndex);
            productTr = newResource.transform;
            productTr.parent = _productsTr[productCount];
            productTr.localPosition = Vector3.zero;
            productTr.localRotation = Quaternion.identity;
            _products.Add(newResource);

            if (productCount == _maxProducts)
                _isFull = true;
        }

        if (!_isFull)
            Invoke(nameof(Produce), _productionTime);
        else
            _isProducing = false;
    }
    public bool TryUseMushroom()
    {
        if (_products.Count < 1)
        {
            if (!_isProducing)
                Invoke(nameof(Produce), _productionTime);

            return false;
        }

        Resource mushroomToUse = _products[^1];
        _products.Remove(mushroomToUse);
        _resourcePool.ReturnResourceToPool(mushroomToUse);

        if (!_isProducing)
            Invoke(nameof(Produce), _productionTime);

        return true;
    }
}
