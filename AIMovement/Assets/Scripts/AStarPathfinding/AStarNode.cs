/*
    ============================================================

    Thsi script was made by Kevin Johansson.

    Additional information.
        This script is called AStarLite Because II don't have 
        enough time build a full A* pathfinding system from 
        scrach.

        We don't inherit from  Monobehaviour here because
        there is no need for us to do it.

    ============================================================
*/

using System;
using System.Collections.Generic;
using UnityEngine; 

public class AStarNode {

    // The position on our grid
    public Vector3Int gridPosition;

    // List containing node neighbours
    public List<AStarNode> neighbours = new List<AStarNode>();

    // Is the node an obsricale?
    public bool isObsticale = false; 

    // Distance from start point to goal 
    public int gCostDistanceFromStart = 0;
    // Distance from node to goal
    public int hCostDistanceFromGoal = 0; 
    // The total cots of moveing to the set grid position
    public int fCostTotal = 0;

    // The order that the nodes was picked in
    public int pickOrder = 0;

    // State to check if the cost has already been calculated
    bool isCostCalculated = false;
    
    // This will be called as our constructor
    public AStarNode(Vector3Int _gridPosition) {
        gridPosition = _gridPosition;
    }
}

