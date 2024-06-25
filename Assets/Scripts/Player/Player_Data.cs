using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EvoType
{ 
    First,
    Second,
    Third
}

public class Player_Data : MonoBehaviour
{
    [SerializeField] private EvoType _evoType = EvoType.First;
    public EvoType @EvoType => _evoType;

    [SerializeField] private Player_HUD _hud;

    [SerializeField] private GameObject[] _evoModels;
    public GameObject[] EvoModels => _evoModels;

    [SerializeField] private int _maxExp = 10;
    public int MaxExp => _maxExp;

    [SerializeField] private int _currentExp = 0;
    public int CurrentExp => _currentExp;

    [SerializeField] private float _expFactor = 1.25f;
    public float ExpFactor => _expFactor;

    [SerializeField] private float _scaleIncrement = 1.0f;
    public float ScaleIncrement => _scaleIncrement;

    [SerializeField] private float _scaleIncrementFactor = 1.5f;
    public float ScaleIncrementFactor => _scaleIncrementFactor;

    [SerializeField] private int _currentLevel = 1;
    public int CurrentLevel => _currentLevel;

    [SerializeField] private int _growthLevelCounter = 1;
    public int GrowthLevelCounter => _growthLevelCounter;

    [SerializeField] private int _growthTreshold = 5;
    public int GrowthTreshold => _growthTreshold;

    [SerializeField] private int _evolveLevelCounter = 1;
    public int EvolveLevelCounter => _evolveLevelCounter;

    [SerializeField] private int _evolveTreshold = 5;
    public int EvolveTreshold => _evolveTreshold;

    private void LevelUp()
    {
        _currentLevel++;
        _growthLevelCounter++;

        float tempMaxExp = _maxExp * _expFactor;
        _maxExp = Mathf.RoundToInt(tempMaxExp);
        _hud.SetNewMaxExp(_maxExp);

        if (_growthLevelCounter >= _growthTreshold)
        {
            EventManager.InvokeGrowth();

            _growthLevelCounter = 0;
            _evolveLevelCounter++;

            float tempScaleIncrement = _scaleIncrement * _scaleIncrementFactor;
            _scaleIncrement = Mathf.RoundToInt(tempScaleIncrement);

            if (_evolveLevelCounter >= _evolveTreshold)
            {
                _evoType = (EvoType)(((int)_evoType + 1) % Enum.GetValues(typeof(EvoType)).Length); // increment enum
                EventManager.InvokeEvolve(_evoType);
            }
        }
    }
    public void GainExp(int newExp)
    {
        _currentExp = newExp;
        _hud.UpdateExpBar(_maxExp, _currentExp);

        if (_currentExp >= _maxExp)
            LevelUp();
    }
    public void IncreaseGrowth(float newScaleIncrement)
    {
        _scaleIncrement = newScaleIncrement;
    }
}
