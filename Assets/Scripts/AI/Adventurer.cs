using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.EnhancedTouch;

public class Adventurer : Character
{
    [Header("Animation")]
    [SerializeField] private Animator _aiAnimator;
    [SerializeField] private float _idleGestureTime = 7.5f;

    private float _idleTime = 0.0f;
    private bool _isGesturing = false;

    [Header("AI Data")]
    [SerializeField] private AdventurerType _type;
    public AdventurerType Type => _type;

    [SerializeField] private Transform _itemPos;
    [SerializeField] private float _lookAtOffset = 2.0f;

    [SerializeField] private NavMeshAgent _agent;
    public NavMeshAgent Agent => _agent;

    private Transform _tradingTr = null;
    public Transform TradingTr { get => _tradingTr; set => _tradingTr = value; }

    private Transform _exitTr = null;
    public Transform ExitTr { get => _exitTr; set => _exitTr = value; }

    private bool _isAlive = false;
    public bool IsAlive { get => _isAlive; set => _isAlive = value; }

    private bool _hasBoughtItem = false;
    public bool HasBoughtItem => _hasBoughtItem;

    private AIObjectPool _aiPool = null;

    private ResourceObjectPool _resourcePool = null;
    private Resource _resource = null;

    private WeaponObjectPool _weaponPool = null;
    private Weapon _weapon = null;

    private const string _resourceCustomerTag = "ResourceCustomer", _weaponCustomerTag = "WeaponCustomer";

    private void OnEnable()
    {
        EventManager.OnHoldResource += OnHoldResource;
    }
    /*private void Start()
    {
        _agent.SetDestination(_tradingTr.position);
        _aiPool = GameManager.Instance.AiPool;
        _resourcePool = GameManager.Instance.ResourcePool;
        _weaponPool = GameManager.Instance.WeaponPool;

        Vector3 lookAtPos = _tradingTr.position;
        lookAtPos.x += _lookAtOffset;
        _agent.transform.LookAt(lookAtPos, Vector3.up);
        _isAlive = true;
    }*/
    private void Update()
    {
        if (!_isAlive)
            return;

        Vector3 scaledMovement = _agent.speed * Time.deltaTime * _agent.velocity.normalized;
        _aiAnimator.SetFloat("Move Speed", scaledMovement.normalized.magnitude);

        if (scaledMovement == Vector3.zero)
        {
            _idleTime += Time.deltaTime;

            if (_idleTime >= _idleGestureTime && !_isGesturing) // idle animation, don't know why I called it Gesturing
            {
                _isGesturing = true;
                _aiAnimator.SetBool("Is Gesturing", _isGesturing);
            }
        }
        else if (_idleTime != 0)
        {
            _idleTime = 0;

            _isGesturing = false;
            _aiAnimator.SetBool("Is Gesturing", _isGesturing);
        }

        // Check if the agent has reached the current target
        if (_isAlive && !_agent.pathPending && _agent.remainingDistance < 0.1f)
        {
            if (!_hasBoughtItem)
                return;

            Vector2 correctDestination = new(_agent.destination.x, _agent.destination.z);
            Vector2 correctTarget = new(_tradingTr.position.x, _tradingTr.position.z);

            if (correctDestination == correctTarget)
            {
                _agent.SetDestination(_exitTr.position);
                _agent.transform.LookAt(_exitTr.position, Vector3.up);
            }
            else if (gameObject.CompareTag(_resourceCustomerTag))
            {
                _isAlive = false;
                _resourcePool.ReturnResourceToPool(_resource);
                _resource = null;
                _hasBoughtItem = false;
                EventManager.InvokeHoldResource(this, false);
                _aiPool.RespawnAdventurer(this);
            }
            else if (gameObject.CompareTag(_weaponCustomerTag))
            {
                _isAlive = false;
                _weaponPool.ReturnWeaponToPool(_weapon);
                _weapon = null;
                _hasBoughtItem = false;
                EventManager.InvokeHoldWeapon(this, false);
                _aiPool.RespawnAdventurer(this);
            }
        }
    }
    private void OnDisable()
    {
        EventManager.OnHoldResource -= OnHoldResource;
    }

    public void StartRemotely()
    {
        _agent.enabled = true;
        _agent.SetDestination(_tradingTr.position);
        _aiPool = GameManager.Instance.AIPool;
        _resourcePool = GameManager.Instance.ResourcePool;
        _weaponPool = GameManager.Instance.WeaponPool;

        Vector3 lookAtPos = _tradingTr.position;
        lookAtPos.x += _lookAtOffset;
        _agent.transform.LookAt(lookAtPos, Vector3.up);
        _isAlive = true;
    }

    private void OnHoldResource(Character chara, bool isHoldingResource) // nned to tweak event
    {
        if (chara is Adventurer && chara as Adventurer == this)
            _aiAnimator.SetBool("Is Holding", isHoldingResource);
    }
    public Object BuyItem(PlayerInventory inventory)
    {
        Object obj = null;
        if (gameObject.CompareTag(_resourceCustomerTag))
        {
            _resource = inventory.PayFirstResource();
            if (!_resource)
                return null;

            _resource.transform.parent = _itemPos;
            _resource.transform.localPosition = Vector3.zero;
            _resource.transform.localRotation = Quaternion.identity;
            obj = _resource;

            EventManager.InvokeHoldResource(this, true);
        }
        else if (gameObject.CompareTag(_weaponCustomerTag))
        {
            _weapon = inventory.GiveWeapon();
            if (!_weapon)
                return null;

            _weapon.transform.parent = _itemPos;
            _weapon.transform.localPosition = Vector3.zero;
            _weapon.transform.localRotation = Quaternion.identity;
            obj = _resource;

            EventManager.InvokeHoldWeapon(this, true);
        }
        _hasBoughtItem = true;
        return obj;
    }
}
