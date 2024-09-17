using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    [SerializeField] protected ObjectiveType _objectiveType; // for quests
    public ObjectiveType ObjectiveType => _objectiveType;

    [SerializeField] protected int _level; // determines in which lvl the player can consume this
    public int Level => _level;

    [SerializeField] protected int _progressionReward; // level progress
    public int ProgressionReward => _progressionReward;

    protected int _reward; // player exp
    public int Reward => _reward;

    protected const float _rewardFactor = 2.0f;
    protected const int _initialExpValue = 10;

    [SerializeField] protected OutlineAltered _outline;
    public OutlineAltered Outline => _outline;

    private void Start()
    {
        CalculateExp();
        //InitializeOutlineShader();

        // quick and dirty
        _progressionReward = 1;
    }
    private void OnDisable()
    {
        SoundManager.Instance.PlayPickupSound();
    }

    private void CalculateExp_Deprecated()
    {
        _reward = (int)(_initialExpValue * _level * _rewardFactor);
    }
    private void CalculateExp()
    {
        _reward = 3 * (int)Mathf.Pow(_rewardFactor, _level - 1);
    }
}
