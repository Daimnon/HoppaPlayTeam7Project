using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshManager : MonoBehaviour
{
    //[SerializeField] private NavMeshSurface _navSurface;

    private void OnEnable()
    {
        EventManager.OnBakeNavMesh += OnBakeNavMesh;
    }
    private void OnDisable()
    {
        EventManager.OnBakeNavMesh -= OnBakeNavMesh;
    }

    private void OnBakeNavMesh()
    {
        //_navSurface.BuildNavMesh();
    }
}
