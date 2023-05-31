/*  
    ============================================================

    The general BT architecture was written by Kevin Johansson.

    Additional info.
        Since this script (task patrol) isn't a monobehaviour 
        (but rather an abstract blob of data), we are forced 
        to pass it which transform to use.

    ============================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class TaskPatrol : Node
{
    private Transform _transform;
    private Transform[] _waypoints;
    private AStarPathfinding _pathfinding; // Reference to your A* script

    private int _currentWaypointIndex = 0;
    private Vector3[] _currentPath;

    private float _waitTime = 1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;

    public TaskPatrol(Transform transform, Transform[] waypoints, AStarPathfinding pathfinding)
    {
        _transform = transform;
        _waypoints = waypoints;
        _pathfinding = pathfinding;
    }

    public override NodeState Evaluate()
    {
        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter < _waitTime)
            {
                _waiting = false;
            }
        }
        else
        {
            Transform wp = _waypoints[_currentWaypointIndex];

            if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
            {
                _transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;

                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
            }
            else if (_currentPath == null || _currentPath.Length == 0)
            {
                Vector3 startPos = _transform.position;
                Vector3 targetPos = wp.position;
                _pathfinding.StartFindPath(startPos, targetPos, OnPathFound);
            }
            else
            {
                MoveToNextWaypoint();
            }
        }

        state = NodeState.RUNNING;
        return state;
    }

    private void OnPathFound(Vector3[] path)
    {
        _currentPath = path;
        MoveToNextWaypoint();
    }

    private void MoveToNextWaypoint()
    {
        if (_currentPath != null && _currentPath.Length > 0)
        {
            Vector3 nextPosition = _currentPath[0];
            _transform.position = Vector3.MoveTowards(_transform.position, nextPosition, RussyBT.speed * Time.deltaTime);
            _transform.LookAt(nextPosition);

            if (Vector3.Distance(_transform.position, nextPosition) < 0.01f)
            {
                // Reached the current waypoint, remove it from the path
                _currentPath = _currentPath[1..];
            }
        }
    }
}