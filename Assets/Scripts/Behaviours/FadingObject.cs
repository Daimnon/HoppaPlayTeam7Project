using System;
using System.Collections.Generic;
using UnityEngine;

public class FadingObject : MonoBehaviour, IEquatable<FadingObject>
{
    [SerializeField] private List<Renderer> _renderers = new();

    private List<Material> _materials = new();
    public List<Material> Materials => _materials;

    private List<float> _origianlAlphas = new();
    public List<float> OriginalAlpha { get => _origianlAlphas; set => _origianlAlphas = value; }

    [SerializeField] private Vector3 _position;
    public Vector3 Position => _position;

    private void Awake()
    {
        _position = transform.position;

        if (_renderers.Count == 0)
            _renderers.AddRange(GetComponentsInChildren<Renderer>());

        foreach (Renderer renderer in _renderers)
            _materials.AddRange(renderer.materials);

        foreach (Material material in _materials)
            _origianlAlphas.Add(material.color.a);
    }

    public bool Equals(FadingObject other)
    {
        return _position.Equals(other.Position);
    }
    public override int GetHashCode()
    {
        return _position.GetHashCode();
    }
}
