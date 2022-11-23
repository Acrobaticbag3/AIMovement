using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode {
    
    public bool isWalkable;               // Two node states, walkable or unwalkable
    public Vector3 nodeWorldPosition;     // What point in the world does the node have?

    public AStarNode(bool _isWalkable, Vector3 _nodeWorldPosition) {      // Assign values to new node
        isWalkable = _isWalkable;
        nodeWorldPosition = _nodeWorldPosition;
    }                                      
}