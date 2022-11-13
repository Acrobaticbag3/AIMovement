/*
    ============================================================

    This script was made by Kevin Johansson.

    Additional information.
        This script is called AStarLite
        Because II don't have enough
        time build a full A* pathfinding
        system from scratch.

        [0] - Since the grid has its own set of coordinates, it
        won't use the coordinates that Unity uses. This Method 
        fixes that by converting the coordinates.

        [1] - Since the destination Vector3 system uses its own 
        set of coordinates. We need to Once again convert the
        coordinates. However, this time in the "opposite" way
        Since we want the world position to mach the grid 
        point.

    ============================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStarLite : MonoBehaviour {

    [Header(header: "Grid creation")]
    int gridSizeX = 40;
    int gridSizeZ = 40;
    float cellSize = 2;

    [Header(header: "Grid Nodes")]
    [SerializeField]
    AStarNode[,] aStarNodes;
    [SerializeField] 
    AStarNode startNode;

    List<AStarNode> nodesToCheck = new List<AStarNode>();
    List<AStarNode> nodesChecked = new List<AStarNode>();

    List<Vector3> aiPath = new List<Vector3>();

    [Header(header: "Debugging")]
    Vector3 startPositionDebug = new Vector3(x: 1000, y: 0, z: 0);
    Vector3 destinationPositionDebug = new Vector3(x: 1000, y: 0, z: 0);

    // Start is called before the first frame update
    void Start() {
        CreateGrid();

        FindOurPath(destination: new Vector3(x: 32, y: 0, z: 17));
    }

    void CreateGrid() {
        // Allocates space in in the array for the nodes
        aStarNodes = new AStarNode[gridSizeX, gridSizeZ];

        for (int x = 0; x < gridSizeX; x++) 
            for (int z = 0; z < gridSizeZ; z++) {

                aStarNodes[x, z] = new AStarNode(_gridPosition: new Vector3Int(x: x, y: 0, z: z));

                Vector3 worldPosition = ConvertGridPositionToWorldPosition(aStarNode: aStarNodes[x, z]);

                // Checks if the A* node is an Obstacle
                Collider[] hitCollider = Physics.OverlapSphere(position: worldPosition, radius: cellSize / 2.0f); 

                if (hitCollider != null) { // [0] - experimental fiture, might break stuff

                    // The ground is obviously not an Obstacle 
                    if (hitCollider[0].CompareTag(tag: "Ground"))
                        continue;

                    // Ignore other ai, they are not Obstacle
                    if (hitCollider[0].CompareTag(tag: "EnemyAI"))
                        continue;

                    // Ignore Player, they are not an Obstacle
                    if (hitCollider[0].CompareTag(tag: "Player"))
                        continue;

                    // Mark as an object
                    aStarNodes[x, z].isObstacle = true;
                }
            }

        // Loop Through the grid again and populate neighbours
        for (int x = 0; x < gridSizeX; x++) 
            for (int z = 0; z < gridSizeZ; z++) {
                
                // Check northern neighbours, if we're on the edge don't add it
                if (z - 1 >= 0) {

                    if (!aStarNodes[x, z -1].isObstacle) 
                        aStarNodes[x, z].neighbours.Add(item: aStarNodes[x, z -1]);
                } 

                // Check Southern neighbours, if we're on the edge don't add it 
                if (z + 1 <= gridSizeZ - 1) {

                    if (!aStarNodes[x, z +1].isObstacle) 
                        aStarNodes[x, z].neighbours.Add(item: aStarNodes[x, z +1]);
                }  

                // Check eastern neighbours, if we're on the edge don't add it
                if (x - 1 >= 0) {

                    if (!aStarNodes[x -1, z].isObstacle) 
                        aStarNodes[x, z].neighbours.Add(item: aStarNodes[x -1, z]);
                } 

                // Check Western neighbours, if we're on the edge don't add it
                if (x + 1 <= gridSizeX - 1) {

                    if (!aStarNodes[x +1, z].isObstacle) 
                        aStarNodes[x, z].neighbours.Add(item: aStarNodes[x +1, z]); 
                }
            }        
    }

    public List<Vector3> FindOurPath(Vector3 destination) {
        if (aStarNodes == null)
            return null;

        // Convert destination from world to grid position
        Vector3Int destinationGridPoint = ConvertWorldToGridPoint(destination);
        Vector3Int currentPositionGridPoint = ConvertWorldToGridPoint(transform.position); 

        // Set a debug position that can be show while developing
        destinationPositionDebug = destination;

        // Start the algorithm by calculating the costs for the first node 
        startNode = GetNodeFromPoint(gridPoint: currentPositionGridPoint);

        // Store the start point so that we can use it while developing
        startPositionDebug = ConvertGridPositionToWorldPosition(aStarNode: startNode);

        // Set the current node to our start node
        AStarNode currentNode = startNode;

        bool isDoneFindingPath = false;
        int pickedOrder = 1;

        while (!isDoneFindingPath) {
            // Remove the current node from the list of nodes that should be checked. 
            nodesToCheck.Remove(currentNode);

            // Set the pick order
            currentNode.pickedOrder = pickedOrder;

            pickedOrder++;

            // Add the current node to the checked list
            nodesChecked.Add(currentNode);

            // Yay! We found the destination
            if (currentNode.gridPosition == destinationGridPoint) {
                isDoneFindingPath = true;
                break;
            }

            // Calculate cost for all nodes
            CalculateCostsForNodeAndNeighbours(currentNode, currentPositionGridPoint, destinationGridPoint);

            // Check if the neighbour nodes should be considered
            foreach (AStarNode neighbourNode in currentNode.neighbours) {
                // Skip any node that has already been checked
                if (nodesChecked.Contains(neighbourNode))
                    continue;

                // Skip any node that is already on the list
                if (nodesToCheck.Contains(neighbourNode))
                    continue;

                // Add the node to the list that we should check 
                nodesToCheck.Add(neighbourNode);
            }

            // Sort the list so that the items with the lowest Total cost (f cost) and if they have the same value then lets pick the one with the lowest cost to reach the goal
            nodesToCheck = nodesToCheck.OrderBy(x => x.fCostTotal).ThenBy(x => x.hCostDistanceFromGoal).ToList();

            // Pick the node with the lowest cost as the next node
            if (nodesToCheck.Count == 0) {

                Debug.LogWarning($"No nodes left in next nodes to check, we have no solution {transform.name}");
                return null;

            } else {
                currentNode = nodesToCheck[0];
            }
        }

        aiPath = CreatePathForAgent(currentPositionGridPoint);

        return null;
    }

    List<Vector3> CreatePathForAgent(Vector3Int currentPositionGridPoint) {
        List<Vector3> aiPathResult = new List<Vector3>();
        List<AStarNode> aiPath = new List<AStarNode>();

        //Reverse the nodes to check as the last added node will be the AI destination
        nodesChecked.Reverse();

        bool isPathCreated = false;

        AStarNode currentNode = nodesChecked[0];

        aiPath.Add(currentNode);

        // Failsafe
        int attempts = 0;

        while (!isPathCreated) {
            
            // go backwards with the lowest creation order
            currentNode.neighbours = currentNode.neighbours.OrderBy(x => x.pickedOrder).ToList();

            foreach (AStarNode aStarNode in currentNode.neighbours) {
                
                if (aiPath.Contains(currentNode) && nodesChecked.Contains(aStarNode)) {

                    aiPath.Add(aStarNode);
                    currentNode = aStarNode;

                    break;
                }
            }

            if (currentNode == startNode) 
                isPathCreated = true;
    
            if (attempts > 1000) {

                Debug.LogWarning("CreatePathForAgent failed after too many attempts");
                break;
            }

            attempts++;
        }

        foreach (AStarNode aStarNode in aiPath) {
            aiPathResult.Add(ConvertGridPositionToWorldPosition(aStarNode));
        }

        // Flipp our result
        aiPathResult.Reverse();

        return aiPathResult;
    }

    void CalculateCostsForNodeAndNeighbours(AStarNode aStarNode, Vector3Int aiPosition, Vector3Int aiDestination) {
        aStarNode.CalculateCostsForNode(aiPosition, aiDestination);

        foreach (AStarNode neighbourNode in aStarNode.neighbours) {
            neighbourNode.CalculateCostsForNode(aiPosition, aiDestination);
        }
    }

    // A helper function that helps us to find our start note.
    AStarNode GetNodeFromPoint(Vector3Int gridPoint) {
        if (gridPoint.x < 0)
            return null;

        if (gridPoint.x > gridSizeX - 1)
            return null;

        if (gridPoint.z < 0)
            return null;

        if (gridPoint.z > gridSizeZ - 1)
            return null;

        return aStarNodes[gridPoint.x, gridPoint.z];
    }

    Vector3Int ConvertWorldToGridPoint(Vector3 position) { // [1]
        Vector3Int gridPoint = new Vector3Int(x: Mathf.RoundToInt(f: position.x / cellSize + gridSizeX / 2.0f), y: Mathf.RoundToInt(f: position.z / cellSize + gridSizeZ / 2.0f));

        return gridPoint;
    }

    Vector3 ConvertGridPositionToWorldPosition(AStarNode aStarNode) { // [0]
        return new Vector3(x: aStarNode.gridPosition.x * cellSize - (gridSizeX * cellSize) / 2.0f, y: 0, z: aStarNode.gridPosition.z * cellSize - (gridSizeZ * cellSize) / 2.0f);
    }

    void OnDrawGizmos() {
        if (aStarNodes == null) 
            return;
        
        // Draw grid for debug reasons
        for (int x = 0; x < gridSizeX; x++) 
            for (int z = 0; z < gridSizeZ; z++) {
                
                if (aStarNodes[x, z].isObstacle)
                    Gizmos.color = Color.red;
                else Gizmos.color = Color.green;

                Gizmos.DrawWireCube(center: ConvertGridPositionToWorldPosition(aStarNode: aStarNodes[x, z]), size: new Vector3(x: cellSize, y: cellSize, z: cellSize));
            }

        foreach (AStarNode checkedNode in nodesChecked) {

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(ConvertGridPositionToWorldPosition(checkedNode), 1.0f);
        }

        foreach (AStarNode toCheckNode in nodesToCheck) {
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(ConvertGridPositionToWorldPosition(toCheckNode), 1.0f);
        }

        Vector3 lastAIPoint = Vector3.zero;
        bool isFirstStep = true;

        Gizmos.color = Color.black;

        foreach (Vector3 point in aiPath) {

            if (!isFirstStep)
                Gizmos.DrawLine(lastAIPoint, point);

            lastAIPoint = point;

            isFirstStep = false;    
        }

        // Draw start position
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(center: startPositionDebug, radius: 1f);

        // Draw end position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(center: destinationPositionDebug, radius: 1f);
    }

}