using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MushroomToPotionConverter : MonoBehaviour
{
    [SerializeField] private MushroomProducer _mushroomClaster;
    [SerializeField] private MeshRenderer[] _potionRenderes;
    [SerializeField] private Material _clearMat, _completeMat;
    [SerializeField] private float _brewTime = 3.0f;
    [SerializeField] private int _potionCount = 3, _clearPotions = 3, _completePotions = 0;

    [SerializeField] private bool _isFull = false;
    public  bool IsFull { get => _isFull; set => _isFull = value; }

    private WaitForSeconds _waitForBrew = null;
    private IEnumerator _makePotions = null;
    private const string _playerTag = "Player";
    private bool _isMakingPotions = false;

    private void Awake()
    {
        _clearMat = new(_clearMat);
        _completeMat = new(_completeMat);
        _waitForBrew = new WaitForSeconds(_brewTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
            StartMakingPotions();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
            StopMakingPotions();
    }

    private void CompletePotion()
    {
        if (_completePotions >= _potionCount)
            return;

        _potionRenderes[_completePotions].material = _completeMat;
        _clearPotions--;
        _completePotions++;
    }
    private void EmptyPotion()
    {
        if (_completePotions < 1)
            return;

        _potionRenderes[_clearPotions].material = _clearMat;
        _completePotions--;
        _clearPotions++;
    }

    private IEnumerator MakePotions()
    {
        if (_mushroomClaster.Products.Count < 1 || _completePotions >= _potionCount)
            yield return new WaitUntil(() => _mushroomClaster.Products.Count > 0 && _completePotions < _potionCount);

        while (_isMakingPotions)
        {
            if (_mushroomClaster.TryUseMushroom())
            {
                CompletePotion();
                yield return _waitForBrew;
            }
            else
            {
                _isMakingPotions = false;
                break;
            }
        }

        _makePotions = null;
        StartMakingPotions();
    }

    public void StartMakingPotions()
    {
        _makePotions = MakePotions();
        _isMakingPotions = true;
        StartCoroutine(_makePotions);
    }
    public void StopMakingPotions()
    {
        StopCoroutine(_makePotions);
        _isMakingPotions = false;
        _makePotions = null;
    }

    public bool TryUsePotion()
    {
        if (_completePotions < 1)
            return false;

        EmptyPotion();
        return true;
    }
}
