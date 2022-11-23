using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour {

    // public Transform player; proof that grid track works
    
    AStarNode[,] grid;                  // 2D Node array representing our grid
    public Vector2 sizeOfWorldGrid;     // Represents the coardinates of our grid
    public float radiusOfNode;          // Size of induvidual node
    public LayerMask unwalkableMask;

    float diamaterOfNode;
    int gridSizeX, gridSizeY;

    void Start() {     
        diamaterOfNode = radiusOfNode * 2;
        gridSizeX = Mathf.RoundToInt(sizeOfWorldGrid.x/diamaterOfNode);     // Gives us the amount of nodes that can fit in our grid for x axis
        gridSizeY = Mathf.RoundToInt(sizeOfWorldGrid.y/diamaterOfNode);     // Gives us the amount of nodes that can fit in our grid for y axis

        CreateGrid();
    }


    void CreateGrid() {
        grid = new AStarNode[gridSizeX,gridSizeY];
        Vector3 bottomLeftOfWorld = transform.position - Vector3.right * sizeOfWorldGrid.x / 2 - Vector3.forward * sizeOfWorldGrid.y / 2; 

        for(int x = 0; x < gridSizeX; x++) {
            for(int y = 0; y < gridSizeY; y++) {
                Vector3 worldPoint = bottomLeftOfWorld + Vector3.right * (x * diamaterOfNode * radiusOfNode) + Vector3.forward * (y * diamaterOfNode * radiusOfNode);       // Get the world position of our node collision
                bool isWalkable = !(Physics.CheckSphere(worldPoint, radiusOfNode, unwalkableMask));                                                                         // Check for collision     
                grid[x,y] = new AStarNode(isWalkable, worldPoint, x, y);
            }
        }
    }
    
    public List<AStarNode> GetNeighbours(AStarNode node) {
        List<AStarNode> neighbours = new List<AStarNode>();

        for (int x = -1; x <= 1; x++) 
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) 
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY  >= 0 && checkY < gridSizeY) {
                    neighbours.Add(grid[checkX,checkY]);
                }
            }

        return neighbours;
    }

    public AStarNode NodeFromWorldPoint(Vector3 nodeWorldPosition) {                                // Finds a specific node, say the one the player is standing on
        float percentX = (nodeWorldPosition.x + sizeOfWorldGrid.x / 2) / sizeOfWorldGrid.x;         // Get position for x
        float percentY = (nodeWorldPosition.z + sizeOfWorldGrid.y / 2) / sizeOfWorldGrid.y;         // Get position for y
        percentX = Mathf.Clamp01(percentX);                                                         // Clamp x between 0 - 1
        percentY = Mathf.Clamp01(percentY);                                                         // Clamp y between 0 - 1

        int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
        return grid[x,y];
    }

    public List<AStarNode> path;
    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(sizeOfWorldGrid.x, 1 ,sizeOfWorldGrid.y));      // Since we are using Vector2 we have to represent z axis with y

        if (grid != null){ 
            // AStarNode playerNode = NodeFromWorldPoint(player.position); // proof that grid track works
            foreach (AStarNode nodes in grid) {
                Gizmos.color = (nodes.isWalkable)?Color.white:Color.red;                                    // Show if node is walkable red = not walkable white = walkable

                /*if (playerNode == nodes) // proof that grid track works
                    Gizmos.color = Color.cyan;*/

                if (path != null)
                    if (path.Contains(nodes))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(nodes.nodeWorldPosition, Vector3.one * (diamaterOfNode-.1f));
            }
        }
    }

}