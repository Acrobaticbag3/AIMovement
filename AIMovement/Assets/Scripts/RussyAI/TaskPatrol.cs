/*  
    The general BT architecture was written by Kevin Johansson.
    Info was gathered from: https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e

    This script helps to implement the 
    generic Behavior Tree architecture 
    (aka BT architecture).
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using UnityEngine.AI;

public class TaskPatrol : Node {
    private Transform _transform;
    private Animator _animator;
    private Transform[] _waypoints;
    NavMeshAgent agent;

    private int _currentWaypointIndex = 0;

    private float _waitTime = 1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;

    // Constructor for 
    public TaskPatrol(NavMeshAgent agent) => this.agent = agent;

    public TaskPatrol(Transform transform, Transform[] waypoints) {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
        _waypoints = waypoints;
    }

    public override NodeState Evaluate() {
        if (_waiting) {

            _waitCounter += Time.deltaTime;
            if(_waitCounter < _waitTime) {
                _waiting = false;
                _animator.SetBool(name: "Walking", value: true);
            }

        } else {

            Transform wp = _waypoints [_currentWaypointIndex];
            
            if (Vector3.Distance(a: _transform.position, b: wp.position) < 0.01f) {
                _transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;

                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                _animator.SetBool(name: "Walking", value: false);

            } else {
                _transform.position = Vector3.MoveTowards(current: _transform.position, target: wp.position, maxDistanceDelta: RussyBT.speed * Time.deltaTime);
                _transform.LookAt(worldPosition: wp.position);
            } 
        }

        state = NodeState.RUNNING;
        return state;
    }
}
