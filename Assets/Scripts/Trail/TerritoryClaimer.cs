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
    [SerializeField] private ExplosionObjectPool _explosionPool;
    [SerializeField] private TrailRenderer _trailRenderer;
    
    [Header("Data")]
    [SerializeField] private LayerMask _detectionLayer;
    [SerializeField] private float _minDistance = 1.0f; // should increase while growing in size
    [SerializeField] private float _intersectDistance;
    [SerializeField] private float _trailGrowFactor = 0.5f;
    [SerializeField] private float _closeAreaColdown = 0.4f;
    private EvoType _evoType;
    private int _currentEvo = 1;
    private float _currentCloseAreaColdown;
    private float _originalTrailTime = 3.0f;
    private float _firePower = 1.0f;
    private bool _isAreaClosed = false;

    private readonly List<Vector3> _trailPoints = new();
    private List<Vector3> _closedTrailPoints = new();
    private List<Vector3> _closedTrailPointsForGizmos = new();
    private float _startingMinDistance;
    
    [Header("VFXs")]
    [SerializeField] private float _timeForExplosionToDieOut = 5.0f;
    [SerializeField] private float _explosionScaleFactor = 6.0f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] _trailClips;
    private int _igniteCouter = 0;

    private int _explosionCount = 0;
    private SoundManager soundManager;
    private List<Flame> _flames = new();

    #region Monobehaviour Callbacks
    private void OnEnable()
    {
        EventManager.OnAreaClosed += OnAreaClosed;
        EventManager.OnGrowth += OnGrowth;
        EventManager.OnEvolve += OnEvolve;
        EventManager.OnUpgrade += OnUpgrade;
    }

    private void Start() 
    {
        soundManager = FindObjectOfType<SoundManager>();
        _intersectDistance = transform.localScale.x;
        _startingMinDistance = _minDistance;
        _currentCloseAreaColdown = _closeAreaColdown;
    }
    private void Update()
    {
        if (!_isAreaClosed)
        {
            TrackPosition();
            CheckForClosedArea();
        }
        else
        {
            _currentCloseAreaColdown -= Time.deltaTime;
            if (_currentCloseAreaColdown <= 0)
            {
                _currentCloseAreaColdown = _closeAreaColdown;
                _isAreaClosed = false;
            }
        }
    }
    private void OnDisable()
    {
        EventManager.OnAreaClosed -= OnAreaClosed;
        EventManager.OnGrowth -= OnGrowth;
        EventManager.OnEvolve -= OnEvolve;
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
            _flames.Add(trailFlame);
            trailFlame.GrowFlame(transform.localScale);

            _igniteCouter++;  
            if (_igniteCouter == 1)
                SoundManager.Instance.PlayPlayerSound(_trailClips[UnityEngine.Random.Range(0, 2)]);
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
                _isAreaClosed = true;
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
    private void RemovePoint(Vector3 point, Flame trailFlame)
    {
        _flames.Remove(trailFlame);
        _trailPoints.Remove(point);
        _flamePool.ReturnFlameToPool(trailFlame);
    }
    private IEnumerator RemovePointDelayed(Vector3 point, Flame trailFlame)
    {
        float elpasedTime = 0.0f;
        while (elpasedTime < _trailRenderer.time)
        {
            if (_isAreaClosed)
            {
                _flames.Remove(trailFlame);
                _trailPoints.Remove(point);
                _flamePool.ReturnFlameToPool(trailFlame);
                break;
            }
            elpasedTime += Time.deltaTime;
            yield return null;
        }
        yield return StartCoroutine(trailFlame.ShrinkFlameRoutine());

        _flames.Remove(trailFlame);
        _trailPoints.Remove(point);
        _flamePool.ReturnFlameToPool(trailFlame);
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
    private IEnumerator ExplosionRoutine(Vector3 midPos)
    {
        ParticleExplosion explosion = _explosionPool.GetParticleExplosionFromPool(midPos);
        explosion.DoExplosion(transform.localScale * _explosionScaleFactor);
        yield return new WaitForSeconds(_timeForExplosionToDieOut);

        _explosionPool.ReturnParticleExplosionToPool(explosion);
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
            Flame flameTrail = _flames[i];
            RemovePoint(_trailPoints[i], flameTrail);
        }

        SoundManager.Instance.Vibrate();
        StartCoroutine(ExplosionRoutine(midPos));

        _trailPoints.Clear();
        _trailRenderer.Clear();
        _flames.Clear();
        Debug.Log("Closed Area!");

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

        switch (_evoType)
        {
            case EvoType.First:
                _trailRenderer.widthMultiplier += _trailGrowFactor;
                break;
            case EvoType.Second:
                _trailRenderer.widthMultiplier += _trailGrowFactor + 0.4f;
                break;
            case EvoType.Third:
                _trailRenderer.widthMultiplier += 2.0f;
                break;
            default:
                break;
        }
    }
    private void OnEvolve(EvoType type)
    {
        _evoType = type;
        _trailGrowFactor *= (int)type + 1;
        switch (type)
        {
            case EvoType.Second:
                _trailGrowFactor += 0.5f;
                break;
            case EvoType.Third:
                _trailGrowFactor -= 0.5f;
                break;
        }
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