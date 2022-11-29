using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class AStarPathfinding : MonoBehaviour {

    public Transform seeker, target;

    AStarGrid grid;

    void Awake  () {
        grid = GetComponent<AStarGrid>();
    }

    void Update() {
        FindPath(startPos: target.position, targetPos: seeker.position);
    }
   
    void FindPath(Vector3 startPos, Vector3 targetPos) {

        Stopwatch sw = new Stopwatch();
        sw.Start();

        AStarNode startNode = grid.NodeFromWorldPoint(nodeWorldPosition: startPos);            // Get world position for our start node
        AStarNode targetNode = grid.NodeFromWorldPoint(nodeWorldPosition: targetPos);          // Get world position for our destination 

        Heap<AStarNode> openSetOfNodes = new Heap<AStarNode>(maxHeapSize: grid.MaxSize);                 // List of the nodes we want to evaluate 
        HashSet<AStarNode> closedSetOfNodes = new HashSet<AStarNode>();                                  // List of nodes we have evaluated
        openSetOfNodes.Add(item: startNode);

        while (openSetOfNodes.Count > 0) {
            AStarNode currentNode = openSetOfNodes.RemoveFirst();
            closedSetOfNodes.Add(item: currentNode);                             // Add current to Node closed

            if (currentNode == targetNode) {
                sw.Stop();
                print(message: "Path found: " + sw.ElapsedMilliseconds + " ms");

                RetraceMainPath(startNode: startNode, endNode: targetNode);         // Return if our currentNode is the targetNode
                return;
            }

            foreach (AStarNode neighbour in grid.GetNeighbours(node: currentNode)) {
                if (!neighbour.isWalkable || closedSetOfNodes.Contains(item: neighbour)) {
                    continue;
                }
                
                int updatedMovementCostToNeighbour = currentNode.gCost + GetDistance(nodeA: currentNode, nodeB: neighbour);           
                if (updatedMovementCostToNeighbour < neighbour.gCost || !openSetOfNodes.Contains(item: neighbour)) {
                    neighbour.gCost = updatedMovementCostToNeighbour; 
                    neighbour.hCost = GetDistance(nodeA: neighbour, nodeB: targetNode);
                    neighbour.parent = currentNode;

                    if (!openSetOfNodes.Contains(item: neighbour))
                        openSetOfNodes.Add(item: neighbour);
                }
            }
        }
    }

    void RetraceMainPath(AStarNode startNode, AStarNode endNode) {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;

        while (currentNode != startNode) {              // Retrace our steps until we reach our start position
            path.Add(item: currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    int GetDistance(AStarNode nodeA, AStarNode nodeB) {                         // Get distance between two nodes
        int distanceOnX = Mathf.Abs(value: nodeA.gridX - nodeB.gridX);
        int distanceOnY = Mathf.Abs(value: nodeA.gridY - nodeB.gridY);

        if (distanceOnX > distanceOnY) 
            return 14 * distanceOnY + 10 * (distanceOnX - distanceOnY);         // Calculation to make sure our nodes have the right grid and world positions
        return 14 * distanceOnX + 10 * (distanceOnY - distanceOnX);             // Calculation to make sure our nodes have the right grid and world positi
    }
    
}
