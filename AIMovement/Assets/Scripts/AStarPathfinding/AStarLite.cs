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

                // Checks for obsicles
                Collider[] hitCollider = Physics.OverlapSphere(worldPosition, cellSize / 2.0f); // May cause errors down the line, be ware


                /* There is something wrong with this snippet of code, not quite sure what.
                // But grid generation is  

                if (hitCollider != null) { // [0] - experimental fiture, might brake stuff

                    // Ignore other ai, they are not obsticles
                    if (hitCollider[0].transform.root.CompareTag("EnemyAI"))
                        continue;

                    // Ignore Player, they are not an obsticle
                    if (hitCollider[0].transform.root.CompareTag("Player"))
                        continue;

                    // Mark as an object
                    aStarNodes[x, z].isObsticale = true;
                }

                */
            }
        
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