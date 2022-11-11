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
    public AStarNode[,] aStarNodes;
    [SerializeField] 
    public AStarNode startNode;

    List<AStarNode> nodesToCheck = new List<AStarNode>();
    List<AStarNode> nodesChecked = new List<AStarNode>();

    [Header(header: "Debugging")]
    Vector3 startPositionDebug = new Vector3(x: 1000, y: 0, z: 0);
    Vector3 destinationPositionDebug = new Vector3(x: 1000, y: 0, z: 0);

    // Start is called before the first frame update
    void Start() {
        CreateGrid();

        FindOurPath(destination: new Vector3(x: 15, y: 0, z: 15));
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

                if (hitCollider != null) { // [0] - experimental fiture, might brake stuff

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

                    if (!aStarNodes[x, z].isObstacle) 
                        aStarNodes[x, z].neighbours.Add(item: aStarNodes[x, z]);
                } 

                // Check Southern neighbours, if we're on the edge don't add it 
                if (z + 1 <= gridSizeZ - 1) {

                    if (!aStarNodes[x, z].isObstacle) 
                        aStarNodes[x, z].neighbours.Add(item: aStarNodes[x, z]);
                }  

                // Check eastern neighbours, if we're on the edge don't add it
                if (x - 1 >= 0) {

                    if (!aStarNodes[x, z].isObstacle) 
                        aStarNodes[x, z].neighbours.Add(item: aStarNodes[x, z]);
                } 

                // Check Western neighbours, if we're on the edge don't add it
                if (x + 1 <= gridSizeX - 1) {

                    if (!aStarNodes[x, z].isObstacle) 
                        aStarNodes[x, z].neighbours.Add(item: aStarNodes[x, z]); 
                }
            }        
    }

    public List<Vector3> FindOurPath(Vector3 destination) {
        if (aStarNodes == null)
            return null;

        // Convert destination from world to grid position
        Vector3Int destinationGridPoint = ConvertWorldToGridPoint(position: destination);
        Vector3Int currentPositionGridPoint = Vector3Int.FloorToInt(v: transform.position); 

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
            nodesToCheck.Remove(currentNode);

            currentNode.pickedOrder = pickedOrder;

            pickedOrder++;

            nodesChecked.Add(currentNode);

            // Are we done?
            if (currentNode.gridPosition == destinationGridPoint) {
                
                // if yes than we found our destination
                isDoneFindingPath  = true;
                break;
            }

            // Calculates cost for all nodes
            CalculateCostsForNodeAndNeighbours(currentNode, currentPositionGridPoint, destinationGridPoint);

            foreach (AStarNode neighbourNode in currentNode.neighbours) {
                
                if (nodesChecked.Contains(neighbourNode))
                    continue;

                if (nodesToCheck.Contains(neighbourNode))
                    continue;

                nodesToCheck.Add(neighbourNode);
            }

            nodesToCheck = nodesToCheck.OrderBy(x => x.fCostTotal).ThenBy(x => x.hCostDistanceFromGoal).ToList();

            if (nodesToCheck.Count == 0) {

                Debug.LogWarning($"No nodes left in next nodes to check, no solution found");
                return null;

            } else {
                currentNode = nodesToCheck[0];
            }
        }

        return null;
    }

    void CalculateCostsForNodeAndNeighbours(AStarNode aStarNode, Vector3Int aiPosition, Vector3Int aiDestination) {
        aStarNode.CalculateCostsForNode(aiPosition, aiDestination);

        foreach (AStarNode neighbourNode in aStarNode.neighbours)
            neighbourNode.CalculateCostsForNode(aiPosition, aiDestination);
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

        foreach (AStarNode nodesChecked in nodesChecked) {

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(ConvertGridPositionToWorldPosition(nodesChecked), 1.0f);
        }

        foreach (AStarNode nodesChecked in nodesToCheck) {
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(ConvertGridPositionToWorldPosition(nodesChecked), 1.0f);
        }

        // Draw start position
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(center: startPositionDebug, radius: 1f);

        // Draw end position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(center: destinationPositionDebug, radius: 1f);
    }

}