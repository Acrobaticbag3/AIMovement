/*
    ============================================================

    Thsi script was made by Kevin Johansson.

    Additional information.
        This script is called AStarLite
        Because II don't have enough
        time build a full A* pathfinding
        system from scrach.

        [0] - Since the grid has its own set of coordinates, it
        wont use the coordinates that Unity uses. This Method 
        fixes that by converting the coordinates.

        [1] - Since the destination Vector3 system uses its own 
        set of coordinates. We need to Once again convert the
        coordinates. However, this time in the "oposite" way
        Since we want the world position to mach the grid 
        point.

    ============================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarLite : MonoBehaviour {

    [Header("Grid creation")]
    int gridSizeX = 40;
    int gridSizeZ = 40;
    float cellSize = 2;

    AStarNode[,] aStarNodes; 

    // Start is called before the first frame update
    void Start() {
        CreateGrid();
    }

    void CreateGrid() {
        // Allocates space in in the array for the nodes
        aStarNodes = new AStarNode[gridSizeX, gridSizeZ];

        for (int x = 0; x < gridSizeX; x++) 
            for (int z = 0; z < gridSizeZ; z++) {

                aStarNodes[x, z] = new AStarNode(new Vector3Int(x, 0, z));

                Vector3 worldPosition = ConvertGridPositionToWorldPosition(aStarNodes[x, z]);

                // Checks if the A* node is an obsicle
                Collider[] hitCollider = Physics.OverlapSphere(worldPosition, cellSize / 2.0f); 

                if (hitCollider != null) { // [0] - experimental fiture, might brake stuff

                    // The ground is obviously not an obsticle 
                    if (hitCollider[0].CompareTag("Ground"))
                        continue;

                    // Ignore other ai, they are not obsticles
                    if (hitCollider[0].CompareTag("EnemyAI"))
                        continue;

                    // Ignore Player, they are not an obsticle
                    if (hitCollider[0].CompareTag("Player"))
                        continue;

                    // Mark as an object
                    aStarNodes[x, z].isObsticale = true;
                }
            }

        // Loop Through the grid again and populate neighbours
        for (int x = 0; x < gridSizeX; x++) 
            for (int z = 0; z < gridSizeZ; z++) {
                
                // Check northern neighbours, if we're on the edge don't add it
                if (z - 1 >= 0 && !aStarNodes[x, z].isObsticale) 
                    aStarNodes[x, z].neighbours.Add(aStarNodes[x, z]); 

                // Check Southern neighbours, if we're on the edge don't add it 
                if (z + 1 >= gridSizeZ - 1 && !aStarNodes[x, z].isObsticale) 
                    aStarNodes[x, z].neighbours.Add(aStarNodes[x, z]); 

                // Check eastern neighbours, if we're on the edge don't add it
                if (x - 1 >= 0 && !aStarNodes[x, z].isObsticale) 
                    aStarNodes[x, z].neighbours.Add(aStarNodes[x, z]); 

                // Check Western neighbours, if we're on the edge don't add it
                if (x + 1 >= gridSizeX - 1 && !aStarNodes[x, z].isObsticale) 
                    aStarNodes[x, z].neighbours.Add(aStarNodes[x, z]); 
            }        
    }

    public List<Vector3> FindOurPath(Vector3 destination) {
        if (aStarNodes == null)
            return null;

        // Convert destination from world to grid position
        Vector3Int destinationGridPoint = ConvertWorldToGridPoint(destination);
        Vector3Int currentPositionGridPoint = ConvertWorldToGridPoint(transform.position);
    }

    Vector3Int ConvertWorldToGridPoint(Vector3 position) { // [1]
        Vector3Int gridPoint = new Vector3Int(Mathf.RoundToInt(position.x / cellSize + gridSizeX / 2.0f), Mathf.RoundToInt(position.z / cellSize + gridSizeZ / 2.0f));

        return gridPoint;
    }

    Vector3 ConvertGridPositionToWorldPosition(AStarNode aStarNode) { // [0]
        return new Vector3(aStarNode.gridPosition.x * cellSize - (gridSizeX * cellSize) / 2.0f, 0, aStarNode.gridPosition.z * cellSize - (gridSizeZ * cellSize) / 2.0f);
    }

    void OnDrawGizmos() {
        if (aStarNodes == null) 
            return;
        
        // Draw grid
        for (int x = 0; x < gridSizeX; x++) 
            for (int z = 0; z < gridSizeZ; z++) {
    
                if (aStarNodes[x, z].isObsticale)
                    Gizmos.color = Color.red;
                else Gizmos.color = Color.green;

                Gizmos.DrawWireCube(ConvertGridPositionToWorldPosition(aStarNodes[x, z]), new Vector3(cellSize, cellSize, cellSize));
            }
    }

}