using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MagicProducer : ResourceProducer
{
    [Header("Product Source")]
    [SerializeField] private ResourceObjectPool _resourcePool;
    public override ResourceObjectPool ResourcePool { get => _resourcePool; set => _resourcePool = value; }

    [Header("Production Details")]
    [SerializeField] private ResourceProducerType _type = ResourceProducerType.MagicFountain;
    public override ResourceProducerType Type => _type;

    [SerializeField] private int _upgradeFactor = 3;
    public override int UpgradeFactor { get => _upgradeFactor; }

    [SerializeField] private int _maxProducts = 3;
    public override int MaxProducts { get => _maxProducts; set => _maxProducts = value; }

    [SerializeField] private float _productionTime = 0.1f;
    public override float ProductionTime => _productionTime;

    [SerializeField] private bool _isFull = false;
    public override bool IsFull { get => _isFull; set => _isFull = value; }

    [Header("Product placements")]
    [SerializeField] private Transform[] _productsTr;
    public override Transform[] ProductsTr => _productsTr;

    [Header("Animation")]
    [SerializeField] private Transform _dropOrigin;

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
        {
            Invoke(nameof(Produce), _productionTime);

            if (productTr)
                StartCoroutine(DropMagic(productTr, _productsTr[productCount]));
        }
    }

    private IEnumerator DropMagic(Transform newProductTr, Transform productTr)
    {
        float elapsedTime = 0f;
        Vector3 originalScale = newProductTr.localScale;

        while (elapsedTime < _productionTime)
        {
            newProductTr.localScale = Vector3.Lerp(Vector3.zero, originalScale, elapsedTime / _productionTime);
            newProductTr.position = Vector3.Lerp(_dropOrigin.position, productTr.position, elapsedTime / _productionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        newProductTr.localScale = originalScale;
        newProductTr.position = productTr.position;
    }
}
