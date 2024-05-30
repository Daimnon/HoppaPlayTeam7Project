using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalProducer : ResourceProducer
{
    [Header("Product Source")]
    [SerializeField] private ResourceObjectPool _resourcePool;
    public override ResourceObjectPool ResourcePool { get => _resourcePool; set => _resourcePool = value; }

    [Header("Production Details")]
    [SerializeField] private ResourceProducerType _type = ResourceProducerType.CoalMine;
    public override ResourceProducerType Type => _type;

    [SerializeField] private int _upgradeFactor = 3;
    public override int UpgradeFactor { get => _upgradeFactor; }

    [SerializeField] private int _maxProducts = 3;
    public override int MaxProducts { get => _maxProducts; set => _maxProducts = value; }

    [SerializeField] private float _productionTime = 4.5f;
    public override float ProductionTime => _productionTime;

    [SerializeField] private bool _isFull = false;
    public override bool IsFull { get => _isFull; set => _isFull = value; }

    [Header("Product placements")]
    [SerializeField] private Transform[] _productsTr;
    public override Transform[] ProductsTr => _productsTr;

    private List<Resource> _products;
    public override List<Resource> Products => _products;

    [Header("Animations")]
    [SerializeField] private Animator _pickaxeAnimator;
    [SerializeField] private float _hitTime = 2.25f;

    private const string _playerTag = "Player";
    private PlayerInventory _playerInventory;
    private WaitForSeconds _timeToHit, _timeToPrepare;
    private bool _isProducing = false;
    private IEnumerator _produce;

    private void Awake()
    {
        _products = new List<Resource>();

        _timeToHit = new(_hitTime);
        _timeToPrepare = new(_productionTime - _hitTime);
    }
    private void Start()
    {
        Initialize();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _playerInventory = other.GetComponent<PlayerInventory>();
            _produce = ProductionSequence();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!_playerInventory)
            return;

        if (!_isProducing && _products.Count < _maxProducts)
        {
            _isProducing = true;
            StartProducing();
            return;
        }

        Debug.Log("Producing or Maxed");
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
            _playerInventory = null;
    }

    public override void Produce()
    {
        if (_isFull)
            return;

        int productCount = _products.Count;
        if (productCount < _maxProducts && _productsTr[productCount].childCount < 1)
        {
            int resourceIndex = (int)_type;
            Resource newResource = _resourcePool.GetResourceFromPool(resourceIndex);
            newResource.transform.position = _productsTr[productCount].position;
            _products.Add(newResource);

            newResource.OriginTrList = _products;

            if (productCount == _maxProducts)
                _isFull = true;
        }
    }

    private IEnumerator ProductionSequence()
    {
        _pickaxeAnimator.SetTrigger("Trigger Animation");
        yield return _timeToHit;

        Produce();
        yield return _timeToPrepare;

        _pickaxeAnimator.ResetTrigger("Trigger Animation");
        _isProducing = false;
        _produce = null;
    }
    private void StartProducing()
    {
        _produce = ProductionSequence();
        StartCoroutine(_produce);
    }
    private void StopProducing()
    {
        StopCoroutine(_produce);
        _produce = null;
    }
}
