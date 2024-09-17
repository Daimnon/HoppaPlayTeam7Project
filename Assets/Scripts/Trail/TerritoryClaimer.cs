using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TerritoryClaimer : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Player_Controller _playerController;
    [SerializeField] private FlameObjectPool _flamePool;
    [SerializeField] private TrailRenderer _trailRenderer;
    
    [Header("Data")]
    [SerializeField] private LayerMask _detectionLayer;
    [SerializeField] private float _minDistance = 1.0f; // should increase while growing in size
    [SerializeField] private float _intersectDistance;
    private float _originalTrailTime = 3.0f;
    private float _firePower = 1.0f;

    private readonly List<Vector3> _trailPoints = new();
    private List<Vector3> _closedTrailPoints = new();
    private List<Vector3> _closedTrailPointsForGizmos = new();
    private float _startingMinDistance;
    
    [Header("VFXs")]
    //[SerializeField] private VisualEffect _explosionVFX;
    [SerializeField] private float _timeForExplosionToDieOut = 5.0f;
    [SerializeField] private float _explosionScaleFactor = 6.0f;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _igniteAudioClip;
    [SerializeField] private AudioClip _explosionAudioClip;
    [SerializeField] private Vector2 _ignitePitch = new (0.5f,1.5f);
    private int _igniteCouter = 0;

    private int _explosionCount = 0;
    private SoundManager soundManager;
    private List<Flame> _flames = new ();

    #region Monobehaviour Callbacks
    private void OnEnable()
    {
        EventManager.OnAreaClosed += OnAreaClosed;
        EventManager.OnGrowth += OnGrowth;
        EventManager.OnUpgrade += OnUpgrade;
        /*_explosionCoroutines = new();
        _explosionVFX.Reinit();
        _explosionVFX.Stop();*/
    }
    private void Start() 
    {
        soundManager = FindObjectOfType<SoundManager>();
        _intersectDistance = transform.localScale.x;
        _startingMinDistance = _minDistance;
    }
    private void Update()
    {
        TrackPosition();
        CheckForClosedArea();
    }
    private void OnDisable()
    {
        EventManager.OnAreaClosed -= OnAreaClosed;
        EventManager.OnGrowth -= OnGrowth;
        EventManager.OnUpgrade -= OnUpgrade;
    }
    #endregion

    #region General Methods
    private void TrackPosition()
    {
        Vector3 currentPosition = transform.position;
        if (_trailPoints.Count == 0 || Vector3.Distance(_trailPoints[_trailPoints.Count - 1], currentPosition) > _minDistance)
        {
            _trailPoints.Add(currentPosition);

            Flame trailFlame = _flamePool.GetFlameFromPool(currentPosition);
            trailFlame.GrowFlame(transform.localScale);
            _flames.Add(trailFlame);

            _audioSource.pitch = UnityEngine.Random.Range(_ignitePitch.x, _ignitePitch.y);

            _igniteCouter++;  
            if (_igniteCouter == 1)
                _audioSource.PlayOneShot(_igniteAudioClip[UnityEngine.Random.Range(0, 2)]);
            else if (_igniteCouter > 2)
                _igniteCouter = 0;

            StartCoroutine(RemovePointDelayed(currentPosition, trailFlame));
        }
    }
    private void CheckForClosedArea()
    {
        if (_trailPoints.Count < 4) return;

        Vector3 currentPosition = transform.position;
        for (int i = 0; i < _trailPoints.Count - 2; i++) // skip recent points
        {
            Vector3 pointA = _trailPoints[i];
            Vector3 pointB = _trailPoints[i + 1];

            if (IsPlayerCrossingLineSegment(pointA, pointB, currentPosition))
            {
                Vector3 intersectionPoint = GetIntersectionPoint(pointA, pointB, currentPosition);
                _closedTrailPoints = GetPointsFromIntersection(_trailPoints, i, intersectionPoint);
                _closedTrailPointsForGizmos = _closedTrailPoints; // gizmos
                Vector3 midPos = CalculateCentroid(_closedTrailPoints);

                Debug.Log("Center of closed area: " + midPos);
                EventManager.InvokeAreaClosed(midPos);
                break;
            }
        }
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
    private bool IsPlayerCrossingLineSegment(Vector3 pointA, Vector3 pointB, Vector3 playerPosition)
    {
        float distanceFromLine = DistanceFromPointToLineSegment(pointA, pointB, playerPosition);
        return distanceFromLine < _intersectDistance;
    }
    private Vector3 GetIntersectionPoint(Vector3 pointA, Vector3 pointB, Vector3 playerPosition)
    {
        // Here, you can calculate the exact intersection point based on the player's movement
        // For now, we'll use a placeholder that returns the midpoint between pointA and pointB
        return (pointA + pointB) / 2f;
    }
    private List<Vector3> GetPointsFromIntersection(List<Vector3> trailPoints, int intersectionIndex, Vector3 intersectionPoint)
    {
        // Create a new list starting from the intersection point
        List<Vector3> closedAreaPoints = new()
        {
            // Add the intersection point first
            intersectionPoint
        };

        // Add all the points from the intersection index onwards
        for (int i = intersectionIndex + 1; i < trailPoints.Count; i++)
        {
            closedAreaPoints.Add(trailPoints[i]);
        }

        // Also include the current position to complete the closed area
        closedAreaPoints.Add(trailPoints[intersectionIndex]); // First point to close the area

        return closedAreaPoints;
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

            float a = p0.x * p1.z - p1.x * p0.z; // shoelace formula
            signedArea += a;
            cx += (p0.x + p1.x) * a;
            cz += (p0.z + p1.z) * a;
        }

        signedArea *= 0.5f;
        cx /= (6f * signedArea);
        cz /= (6f * signedArea);
        
        return new Vector3(cx, transform.position.y, cz);
    }

    private float DistanceFromPointToLineSegment(Vector3 pointA, Vector3 pointB, Vector3 playerPosition)
    {
        Vector3 lineDirection = pointB - pointA;
        Vector3 playerToA = playerPosition - pointA;

        float t = Vector3.Dot(playerToA, lineDirection) / Vector3.Dot(lineDirection, lineDirection);
        t = Mathf.Clamp01(t);

        Vector3 closestPoint = pointA + t * lineDirection;
        return Vector3.Distance(playerPosition, closestPoint);
    }
    private Bounds CalculateBoundingBox(List<Vector3> points)
    {
        if (points == null || points.Count == 0)
            return new Bounds(Vector3.zero, Vector3.zero);

        Vector3 min = points[0];
        Vector3 max = points[0];

        foreach (var point in points)
        {
            min = Vector3.Min(min, point);
            max = Vector3.Max(max, point);
        }

        // y value to be zero for not missing objects
        min.y = 0;

        // calculate the center and size with new min Y
        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;

        return new Bounds(center, size);
    }
    private IEnumerator RemovePointDelayed(Vector3 point, Flame trailFlame)
    {
        yield return new WaitForSeconds(_trailRenderer.time);

        _flames.Remove(trailFlame);
        _flamePool.ReturnFlameToPool(trailFlame);
        _trailPoints.Remove(point);
    }
    private List<Consumable> GetConsumablesInClosedArea()
    {
        List<Consumable> consumablesInClosedArea = new();

        Bounds bounds = CalculateBoundingBox(_closedTrailPoints);
        Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, _detectionLayer);

        foreach (var collider in colliders)
        {
            if (IsPointInPolygon(collider.transform.position, _closedTrailPoints) && collider.TryGetComponent(out Consumable consumable))
                consumablesInClosedArea.Add(consumable);
        }

        _closedTrailPoints = new();
        return consumablesInClosedArea;
    }
    #endregion

    #region VFX Methods
    /*private void ResetExplosion()
    {
        _explosionVFX.Reinit();
        _explosionVFX.Stop();
        _explosionVFX.transform.SetParent(transform);
    }*/
    /*private void PlayExplosion(Vector3 pos)
    {
        _explosionVFX.transform.SetParent(null);
        _explosionVFX.transform.position = pos;
        _explosionVFX.transform.localScale = transform.localScale / 2f;
        _explosionVFX.Play();
    }*/
    private IEnumerator ExplosionRoutine(Vector3 midPos)
    {
        //PlayExplosion(midPos);
        Flame explosionFlame = _flamePool.GetFlameFromPool(midPos);
        explosionFlame.DoExplosion(transform.localScale * _explosionScaleFactor);
        _audioSource.PlayOneShot(_explosionAudioClip);
        yield return new WaitForSeconds(_timeForExplosionToDieOut);
        yield return explosionFlame.EndExplosion();

        explosionFlame.ResetExplosion();
        _flamePool.ReturnFlameToPool(explosionFlame);
    }
    #endregion

    #region Upgrades
    private void IncreaseFirePower(float firePower)
    {
        _firePower = firePower + 1;
        _trailRenderer.time = _originalTrailTime * (_firePower / 1.99f);
    }
    #endregion

    #region Events
    private void OnAreaClosed(Vector3 midPos)
    {
        List<Consumable> consumablesInClosedArea = GetConsumablesInClosedArea();
        for (int i = 0; i < consumablesInClosedArea.Count; i++)
        {
            _playerController.ConsumeObjectFromExplosion(consumablesInClosedArea[i]);
        }

        for (int i = 0; i < _flames.Count; i++)
        {
            _flamePool.ReturnFlameToPool(_flames[i]);
        }

        StartCoroutine(ExplosionRoutine(midPos));

        _trailPoints.Clear();
        _trailRenderer.Clear();
        _flames.Clear();
        Debug.Log("Closed shape detected!");

        soundManager.PlayFireExplosionSound();
        _explosionCount++;

        if (_explosionCount >= 3)
        {
            EventManager.InvokeObjectiveTrigger3();
            Debug.Log("Objective 3 completed with explosion count: " + _explosionCount);
        }
    }
    private void OnGrowth()
    {
        _minDistance = _startingMinDistance * transform.localScale.x;
        _intersectDistance = transform.localScale.x;
    }
    private void OnUpgrade(UpgradeType type)
    {
        if (type == UpgradeType.FirePower)
            IncreaseFirePower(_firePower);
    }
    #endregion

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // draw lines between the trail points
        for (int i = 0; i < _trailPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(_trailPoints[i], _trailPoints[i + 1]);
        }

        // draw line from the last trail point to the player's current position
        if (_trailPoints.Count > 0)
            Gizmos.DrawLine(_trailPoints[_trailPoints.Count - 1], transform.position);

        // draw spheres at each trail point
        Gizmos.color = Color.blue;
        foreach (var point in _trailPoints)
        {
            Gizmos.DrawSphere(point, 0.1f);
        }

        // draw green lines connecting the points in the closed area loop
        if (_closedTrailPointsForGizmos != null && _closedTrailPointsForGizmos.Count > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _closedTrailPointsForGizmos.Count - 1; i++)
            {
                Gizmos.DrawLine(_closedTrailPointsForGizmos[i], _closedTrailPointsForGizmos[i + 1]);
            }

            // draw line to close the loop
            Gizmos.DrawLine(_closedTrailPointsForGizmos[_closedTrailPointsForGizmos.Count - 1], _closedTrailPointsForGizmos[0]);
        }
    }
}