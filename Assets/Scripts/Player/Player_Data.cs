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

    [SerializeField] private int _maxExp = 100;
    public int MaxExp => _maxExp;

    [SerializeField] private int _currentExp = 0;
    public int CurrentExp => _currentExp;

    [SerializeField] private float _expFactor = 2.1f;
    public float ExpFactor => _expFactor;

    [SerializeField] private float _scaleIncrement = 1.0f;
    public float ScaleIncrement => _scaleIncrement;

    [SerializeField] private float _scaleIncrementFactor = 1.25f;
    public float ScaleIncrementFactor => _scaleIncrementFactor;

    [SerializeField] private int _currentLevel = 1;
    public int CurrentLevel => _currentLevel;

    [SerializeField] private int _maxGrowth = 20;
    public int MaxGrowth => _maxGrowth;

    [SerializeField] private int _growthLevelCounter = 1;
    public int GrowthLevelCounter => _growthLevelCounter;

    [SerializeField] private int _growthTreshold = 1;
    public int GrowthTreshold => _growthTreshold;

    [SerializeField] private int _evolveLevelCounter = 1;
    public int EvolveLevelCounter => _evolveLevelCounter;

    [SerializeField] private int _evolveTreshold = 5;
    public int EvolveTreshold => _evolveTreshold;

    private void OnEnable()
    {
        EventManager.OnUpgrade += OnUpgrade;
        EventManager.OnMultipleGrowth += OnMultipleLevelUp;
    }
    private void OnDisable()
    {
        EventManager.OnUpgrade -= OnUpgrade;
        EventManager.OnMultipleGrowth -= OnMultipleLevelUp;
    }

    private void LevelUp()
    {
        _currentLevel++;
        _growthLevelCounter++;
        _hud.SetNewLevel(_currentLevel);

        float tempCurrentExp = _currentExp - _maxExp;
        float tempMaxExp = _maxExp * _expFactor;
        _maxExp = Mathf.RoundToInt(tempMaxExp);
        _hud.SetNewMaxExp(_maxExp);
        
        _currentExp = Mathf.RoundToInt(tempCurrentExp);
        _hud.UpdateExpBar(_maxExp, _currentExp);

        if (_growthLevelCounter >= _growthTreshold)
        {
            EventManager.InvokeGrowth();

            _growthLevelCounter = 0;
            _evolveLevelCounter++;

            float tempScaleIncrement = _scaleIncrement * _scaleIncrementFactor;
            _scaleIncrement = Mathf.RoundToInt(tempScaleIncrement);

            if (_evolveLevelCounter >= _evolveTreshold)
            {
                int currentValue = (int)_evoType;
                int maxValue = Enum.GetValues(typeof(EvoType)).Length - 1;

                _evoType = currentValue < maxValue ? (EvoType)(currentValue + 1) : _evoType;
                EventManager.InvokeEvolve(_evoType);
                _evolveLevelCounter = 0;

                //_evoType = (EvoType)(((int)_evoType + 1) % Enum.GetValues(typeof(EvoType)).Length); // increment enum
            }
        }

        if (_currentLevel >= 5)
        {
            EventManager.InvokeObjectiveTrigger1();
            Debug.Log("Player reached level 5, objective 1 completed.");
        }

        if (_growthLevelCounter > _maxGrowth)
        {
            //EventManager.InvokeGrowthMaxed();
            return;
        }

        CheckLevelUp();
    }
    private void CheckLevelUp()
    {
        if (_currentExp >= _maxExp)
            LevelUp();
    }
    public void GainExp(int expToGain)
    {
        _currentExp += expToGain;
        _hud.UpdateExpBar(_maxExp, _currentExp); // should be event

        CheckLevelUp();
    }

    #region Events
    private void OnUpgrade(UpgradeType type)
    {
        if (type == UpgradeType.Grow)
            GainExp(_maxExp);
    }
    private void OnMultipleLevelUp(int targetLevel)
    {
        double expToGain = _maxExp * (Math.Pow(_expFactor, targetLevel) - 1) / (_expFactor - 1);
        GainExp(Mathf.RoundToInt((float)expToGain));
    }
    #endregion
}
