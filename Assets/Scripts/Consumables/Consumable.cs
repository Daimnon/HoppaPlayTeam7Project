using UnityEngine;

public class Consumable : MonoBehaviour
{
    [SerializeField] protected int _reward;
    public int Reward => _reward;
}
