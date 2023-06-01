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

public class TaskPatrol : Node {
    private Transform _transform;
    private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;
    private float _waitTime = 1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;
    private AStarAgent _aStarAgent;

    public TaskPatrol(Transform transform, Transform[] waypoints, AStarAgent aStarAgent) {
        _transform = transform;
        _waypoints = waypoints;
        _aStarAgent = aStarAgent;
    }

    public override NodeState Evaluate() {
        if (_waiting) {
            _waitCounter += Time.deltaTime;
            if (_waitCounter < _waitTime) {
                _waiting = false;
            }
        } else {
            Transform wp = _waypoints[_currentWaypointIndex];

            if (Vector3.Distance(_transform.position, wp.position) < 0.01f) {
                _transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;

                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                _aStarAgent.SetTarget(wp); // Set the AStarAgent's target to the current waypoint
            } else {
                _aStarAgent.SetTarget(wp); // Set the AStarAgent's target to the current waypoint
            }
        }

        state = NodeState.RUNNING;
        return state;
    }
}