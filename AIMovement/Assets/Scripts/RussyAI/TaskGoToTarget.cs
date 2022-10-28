/*  
    The general BT architecture was written by Kevin Johansson.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskGoToTarget : Node {
    private Transform _transform;
    private float destination;

    public TaskGoToTarget(Transform transform) {
        _transform = transform;
    }

    public override NodeState Evaluate() {
        Transform target = (Transform)GetData("target");

        if (Vector3.Distance(_transform.position, target.position) >0.01f) {
            _transform.position = Vector3.MoveTowards(_transform.position, target.position, RussyBT.speed * Time.deltaTime);
            _transform.LookAt(target.position);
        }

        state = NodeState.RUNNING;
        return state;
    }
}
