using UnityEngine;

public class Consumable : MonoBehaviour
{
    [SerializeField] protected ObjectiveType _objectiveType;
    public ObjectiveType ObjectiveType => _objectiveType;

    [SerializeField] protected int _level;
    public int Level => _level;

    [SerializeField] protected int _progressionReward;
    public int ProgressionReward => _progressionReward;

    protected int _reward;
    public int Reward => _reward;

    protected const float _rewardFactor = 1.4f;
    protected const int _initialExpValue = 10;

    private void Start()
    {
        _reward = (int)(_initialExpValue * _level * _rewardFactor);
    }
}
