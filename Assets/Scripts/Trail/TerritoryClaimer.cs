using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryClaimer : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject cube;
    [SerializeField] private TrailRenderer _trailRenderer;

    [Header("Data")]
    [SerializeField] private LayerMask _detectionLayer;
    [SerializeField] private float _minDistance = 2.0f; // should increase while growing in size

    private readonly List<Vector3> _trailPoints = new();

    #region Monobehaviour Callbacks
    private void OnEnable()
    {
        EventManager.OnAreaClosed += OnAreaClosed;
    }
    private void Update()
    {
        TrackPosition();
        CheckForClosedArea();
    }
    private void OnDisable()
    {
        EventManager.OnAreaClosed -= OnAreaClosed;
    }
    #endregion

    #region General Methods
    private void TrackPosition()
    {
        Vector3 currentPosition = transform.position;
        if (_trailPoints.Count == 0 || Vector3.Distance(_trailPoints[_trailPoints.Count - 1], currentPosition) > _minDistance)
        {
            _trailPoints.Add(currentPosition);
            StartCoroutine(RemovePointDelayed(currentPosition));
        }
    }
    private void CheckForClosedArea()
    {
        if (_trailPoints.Count < 3) return;

        Vector3 currentPosition = transform.position;
        if (Vector3.Distance(_trailPoints[0], currentPosition) < _minDistance)
        {
            Vector3 midPos = CalculateCentroid(_trailPoints);
            Debug.Log("Centroid of territory" + midPos);

            EventManager.InvokeAreaClosed(CalculateCentroid(_trailPoints));
        }
    }
    private Vector3 CalculateCentroid(List<Vector3> points)
    {
        float signedArea = 0f;
        float cx = 0f;
        float cz = 0f;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p0 = points[i];
            Vector3 p1 = points[(i + 1) % points.Count];

            float a = p0.x * p1.z - p1.x * p0.z;
            signedArea += a;
            cx += (p0.x + p1.x) * a;
            cz += (p0.z + p1.z) * a;
        }

        signedArea *= 0.5f;
        cx /= (6f * signedArea);
        cz /= (6f * signedArea);
        
        return new Vector3(cx, transform.position.y, cz);
    }

    private Bounds CalculateBoundingBox(List<Vector3> points)
    {
        Vector3 min = points[0];
        Vector3 max = points[0];

        foreach (var point in points)
        {
            min = Vector3.Min(min, point);
            max = Vector3.Max(max, point);
        }

        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;

        return new Bounds(center, size);
    }
    private bool IsPointInPolygon(Vector3 point, List<Vector3> polygon)
    {
        bool isInside = false;
        int j = polygon.Count - 1;
        for (int i = 0; i < polygon.Count; j = i++)
        {
            if (((polygon[i].z > point.z) != (polygon[j].z > point.z)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.z - polygon[i].z) / (polygon[j].z - polygon[i].z) + polygon[i].x))
            {
                isInside = !isInside;
            }
        }
        return isInside;
    }
    private IEnumerator RemovePointDelayed(Vector3 point)
    {
        yield return new WaitForSeconds(_trailRenderer.time);
        _trailPoints.Remove(point);
    }
    private List<GameObject> GetObjectsInClosedArea()
    {
        List<GameObject> objectsInClosedArea = new List<GameObject>();

        // Calculate bounding box
        Bounds bounds = CalculateBoundingBox(_trailPoints);

        // Use Physics.OverlapBox to get all colliders in the bounding box
        Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, _detectionLayer);

        foreach (var collider in colliders)
        {
            if (IsPointInPolygon(collider.transform.position, _trailPoints))
            {
                objectsInClosedArea.Add(collider.gameObject);
            }
        }

        return objectsInClosedArea;
    }
    #endregion

    #region Events
    private void OnAreaClosed(Vector3 midPos) // need to carry on from here
    {
        if (GetObjectsInClosedArea().Count > 0)
            Debug.Log(GetObjectsInClosedArea()[0].name);
        
        _trailPoints.Clear();
        _trailRenderer.Clear();
        Debug.Log("Closed shape detected!");
    }
    #endregion

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // Draw lines between the trail points
        for (int i = 0; i < _trailPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(_trailPoints[i], _trailPoints[i + 1]);
        }

        // Draw line from the last trail point to the player's current position
        if (_trailPoints.Count > 0)
        {
            Gizmos.DrawLine(_trailPoints[_trailPoints.Count - 1], transform.position);
        }

        // Draw spheres at each trail point
        Gizmos.color = Color.blue;
        foreach (var point in _trailPoints)
        {
            Gizmos.DrawSphere(point, 0.1f);
        }

        // If a closed area is detected, draw green lines connecting the points in the loop
        if (_trailPoints.Count > 2 && Vector3.Distance(_trailPoints[0], transform.position) < _minDistance)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _trailPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(_trailPoints[i], _trailPoints[i + 1]);
            }
            Gizmos.DrawLine(_trailPoints[_trailPoints.Count - 1], _trailPoints[0]);
        }
    }
}