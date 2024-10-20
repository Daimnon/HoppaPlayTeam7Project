using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Consumable : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected NavMeshObstacle _obstacleNav;

    [SerializeField] protected OutlineAltered _outline;
    public OutlineAltered Outline => _outline;

    [Header("Data")]
    [SerializeField] protected ObjectiveType _objectiveType; // for quests
    public ObjectiveType ObjectiveType => _objectiveType;

    [SerializeField] protected int _level; // determines in which lvl the player can consume this
    public int Level => _level;

    [SerializeField] protected int _progressionReward; // level progress
    public int ProgressionReward => _progressionReward;

    protected const float _rewardFactor = 2.0f;
    protected const int _initialExpValue = 10;

    protected int _reward; // player exp
    public int Reward => _reward;

    private void Start()
    {
        CalculateExp();

        // quick and dirty
        _progressionReward = 1;

        if (!_obstacleNav)
        {
            if (TryGetComponent(out NavMeshObstacle navmesh))
                _obstacleNav = navmesh;
            else
                _obstacleNav = gameObject.AddComponent<NavMeshObstacle>();
        }
    }

    private void CalculateExp_Deprecated()
    {
        _reward = (int)(_initialExpValue * _level * _rewardFactor);
    }
    private void CalculateExp()
    {
        _reward = 3 * (int)Mathf.Pow(_rewardFactor, _level - 1);
    }

    public void AdaptCollision(Player_Data data)
    {
        if (!_obstacleNav) return;
        if (data.CurrentLevel >= _level) Destroy(_obstacleNav);
    }
}
