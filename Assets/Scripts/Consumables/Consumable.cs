using UnityEngine;

public class Consumable : MonoBehaviour
{
    [SerializeField] protected ObjectiveType _objectiveType;
    public ObjectiveType ObjectiveType => _objectiveType;

    [SerializeField] protected int _progressionReward;
    public int ProgressionReward => _progressionReward;

    [SerializeField] protected int _reward;
    public int Reward => _reward;
}
