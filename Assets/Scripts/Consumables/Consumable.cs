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

    protected const float _rewardFactor = 1.4f;
    protected const int _initialExpValue = 10;

    [Header("Outline Properties")]
    [SerializeField] private MeshRenderer _rendererToOutline;
    [SerializeField] private Material _originalOutlineMat;
    private Material[] _originalMaterials;
    private Material[] _renderingMaterials;
    private List<Material> _renderingMaterialsList;
    private Material _outlineMat;

    private void Start()
    {
        CalculateExp();
        InitializeOutlineShader();
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
        _reward = 3 * (int)Mathf.Pow(2, _level - 1);
    }
    private void InitializeOutlineShader()
    {
        _outlineMat = new Material(_originalOutlineMat);

        int rendererToOutlineMaterialArrayLenght = _rendererToOutline.materials.Length;
        _originalMaterials = new Material[rendererToOutlineMaterialArrayLenght];

        for (int i = 0; i < rendererToOutlineMaterialArrayLenght; i++)
        {
            _originalMaterials[i] = _rendererToOutline.materials[i];
        }

        _renderingMaterialsList = new(rendererToOutlineMaterialArrayLenght + 1);

        for (int i = 0; i < rendererToOutlineMaterialArrayLenght; i++)
        {
            _renderingMaterialsList.Add(_rendererToOutline.materials[i]);
        }

        _renderingMaterialsList.Add(_outlineMat);
        _renderingMaterials = _renderingMaterialsList.ToArray();
    }
    public void ApplyOutline()
    {
        _rendererToOutline.materials = _renderingMaterials;
    }
    public void RemoveOutline()
    {
        if (!_rendererToOutline)
            return;

        _rendererToOutline.materials = _originalMaterials;
    }
}
