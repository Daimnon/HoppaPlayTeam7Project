using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FuelStove : ResourceProducerAddon
{
    [SerializeField] private ResourceObjectPool _resourceObjectPool;

    private const string _playerTag = "Player";
    private PlayerInventory _playerInventory;

    [Header("Converted Coal")]
    [SerializeField] private int _convertedCoal = 0;
    public int ConvertedCoal  => _convertedCoal;

    [SerializeField] private float _productionTime = 0.5f;
    public float ProductionTime => _productionTime;

    [Header("Coal")]
    [SerializeField] private int _maxCoal = 3;
    public int MaxCoal { get => _maxCoal; set => _maxCoal = value; }

    private List<Resource> _coal;
    public List<Resource> Coal => _coal;

    private void Awake()
    {
        _coal = new List<Resource>();
    }
    private void OnEnable()
    {
        EventManager.OnForgeUnlocked += OnForgeUnlocked;
    }
    private void Start()
    {
        Initialize();
        ConvertCoal();
    }
    private void OnDisable()
    {
        EventManager.OnForgeUnlocked -= OnForgeUnlocked;
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

        if (_coal.Count < _maxCoal)
            TakeCoal();
        else
            Debug.Log("Max convered coal reached!");
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
            _playerInventory = null;
    }

    private void Initialize()
    {
        if (!_resourceObjectPool && GameManager.Instance.ResourcePool)
            _resourceObjectPool = GameManager.Instance.ResourcePool;
    }
    private void TakeCoal()
    {
        Resource coal = _playerInventory.PayResource(ResourceType.Coal);

        if (!coal)
            return;

        _coal.Add(coal);
        _resourceObjectPool.ReturnResourceToPool(coal);
    }

    public void ConvertCoal() // need to modify for fuel usage
    {
        if (_coal.Count < 1 || _convertedCoal >= _maxCoal)
        {
            StartCoroutine(WaitForCoal());
            return;
        }

        _convertedCoal++;
        _coal.RemoveAt(0);
        // do burning vfx

        Invoke(nameof(ConvertCoal), _productionTime);
    }
    public void UseConvertedCoal()
    {
        _convertedCoal--;
    }
    private IEnumerator WaitForCoal()
    {
        yield return new WaitUntil(()=> _coal.Count > 0);
        ConvertCoal();
    }

    private void OnForgeUnlocked(IronProducer forge)
    {
        forge.Engine = this;
    }
}
