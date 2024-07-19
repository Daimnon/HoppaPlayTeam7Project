using System.Collections.Generic;
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

    [Header("Outline Properties")]
    [SerializeField] private MeshRenderer _rendererToOutline;
    [SerializeField] private Material _originalOutlineMat;
    private Material[] _originalMaterials;
    private Material[] _renderingMaterials;
    private List<Material> _renderingMaterialsList;
    private Material _outlineMat;

    private void Start()
    {
        _reward = (int)(_initialExpValue * _level * _rewardFactor);
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
        _rendererToOutline.materials = _originalMaterials;
    }
}
