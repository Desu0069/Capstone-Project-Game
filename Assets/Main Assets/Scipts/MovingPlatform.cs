using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private WaypointPath _waypointPath;

    [SerializeField]
    private float _speed;

    [SerializeField] private bool _fallAfterFinalWaypoint = false;
    [SerializeField] private float _fallDelay = 1f;
    [SerializeField] private float _fallSpeed = 3f;
    [SerializeField] private bool _destroyAfterFall = false;
    [SerializeField] private float _destroyDelay = 5f;


    private int _targetWaypointIndex;

    private Transform _previousWaypoint;
    private Transform _targetWaypoint;

    private float _timeToWaypoint;
    private float _elapsedTime;

    private bool _isFalling = false;
    private bool _reachedFinalWaypoint = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TargetNextWaypoint();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isFalling)
        {
            transform.position += Vector3.down * _fallSpeed * Time.fixedDeltaTime;
            return;
        }

        _elapsedTime += Time.deltaTime;

        float elapsedPercentage = _elapsedTime / _timeToWaypoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        transform.position = Vector3.Lerp(_previousWaypoint.position, _targetWaypoint.position, elapsedPercentage);

        if (elapsedPercentage >= 1)
        {
            // Check if final waypoint reached
            if (_fallAfterFinalWaypoint && _waypointPath.IsLastWaypoint(_targetWaypointIndex))
            {
                if (!_reachedFinalWaypoint)
                {
                    _reachedFinalWaypoint = true;
                    Invoke(nameof(StartFalling), _fallDelay);
                }
            }
            else
            {
                TargetNextWaypoint();
            }
        }

    }

    private void StartFalling()
    {
        _isFalling = true;

        if (_destroyAfterFall)
        {
            Destroy(gameObject, _destroyDelay);
        }
    }


    private void TargetNextWaypoint()
    {
        _previousWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
        _targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);

        _elapsedTime = 0;

        float distanceToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position);
        _timeToWaypoint = distanceToWaypoint / _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }
}
