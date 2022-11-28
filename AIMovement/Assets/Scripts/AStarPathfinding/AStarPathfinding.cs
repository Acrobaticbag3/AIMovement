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
        FindPath(target.position, seeker.position);
    }
   
    void FindPath(Vector3 startPos, Vector3 targetPos) {

        Stopwatch sw = new Stopwatch();
        sw.Start();

        AStarNode startNode = grid.NodeFromWorldPoint(startPos);            // Get world position for our start node
        AStarNode targetNode = grid.NodeFromWorldPoint(targetPos);          // Get world position for our destination 

        Heap<AStarNode> openSetOfNodes = new Heap<AStarNode>(grid.MaxSize);                 // List of the nodes we want to evaluate 
        HashSet<AStarNode> closedSetOfNodes = new HashSet<AStarNode>();         // List of nodes we have evaluated
        openSetOfNodes.Add(startNode);

        while (openSetOfNodes.Count > 0) {
            AStarNode currentNode = openSetOfNodes.RemoveFirst();
            closedSetOfNodes.Add(currentNode);          // Add current to Node closed

            if (currentNode == targetNode) {
                sw.Stop();
                print("Path found: " + sw.ElapsedMilliseconds + " ms");

                RetraceMainPath(startNode, targetNode);              // Return if our currentNode is the targetNode
                return;
            }

            foreach (AStarNode neighbour in grid.GetNeighbours(currentNode)) {
                if (!neighbour.isWalkable || closedSetOfNodes.Contains(neighbour)) {
                    continue;
                }
                
                int updatedMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);           
                if (updatedMovementCostToNeighbour < neighbour.gCost || !openSetOfNodes.Contains(neighbour)) {
                    neighbour.gCost = updatedMovementCostToNeighbour; 
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSetOfNodes.Contains(neighbour))
                        openSetOfNodes.Add(neighbour);
                }
            }
        }
    }

    void RetraceMainPath(AStarNode startNode, AStarNode endNode) {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;

        while (currentNode != startNode) {          // Retrace our steps untill we reach our start position
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    // Get distance between two nodes
    int GetDistance(AStarNode nodeA, AStarNode nodeB) {
        int distanceOnX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceOnY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distanceOnX > distanceOnY) 
            return 14 * distanceOnY + 10 * (distanceOnX - distanceOnY);
        return 14 * distanceOnX + 10 * (distanceOnY - distanceOnX);
    }
    
}
