using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode {
    
    public bool isWalkable;               // Two node states, walkable or unwalkable
    public Vector3 nodeWorldPosition;     // What point in the world does the node have?
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public AStarNode parent; 

    public AStarNode(bool _isWalkable, Vector3 _nodeWorldPosition, int _gridX, int _gridY) {      // Assign values to new node
        isWalkable = _isWalkable;
        nodeWorldPosition = _nodeWorldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }     

    public int fCost {                  // We get fCost through gCost + hCost, thats why we don't set any values
        get {
            return gCost + hCost;
        }
    }                                 
}