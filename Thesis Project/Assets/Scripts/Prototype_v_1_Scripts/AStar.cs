using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prototype_v_1_Scripts;
using UnityEngine;
using Priority_Queue;

public class AStar : MonoBehaviour
{
    public class Node
    {
        public Vector2Int position;
        public int gCost;                   // Cost from starting node
        public int hCost;                   // Cost to target node
        public int fCost => gCost + hCost;  // Total cost
        public Node parent;                 // For tracing back the path
        
        public Node(Vector2Int position)
        {
            this.position = position;
        }
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        SimplePriorityQueue<Node> openList = new SimplePriorityQueue<Node>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();
        Node startNode = new Node(start);
        Node targetNode = new Node(target);
        
        openList.Enqueue(startNode, startNode.fCost); // Add the starting node to the open list

        while (openList.Count > 0)
        {
            Node currentNode = openList.Dequeue();  // Get the node with the lowest fCost

            // If the current node is the target node, return the path
            if (currentNode.position == target)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                
                Node current = currentNode;
                
                while (current != null)
                {
                    path.Add(current.position);
                    current = current.parent;
                }
                
                path.Reverse(); // Reverse the path to get the correct order
                return path;    // Return the path
            }
            
            // Add current node to closed list after processing
            closedList.Add(currentNode.position);
            
            List<Vector2Int> neighbours = GetNeighbours(currentNode.position);

            foreach (Vector2Int neighbourPosition in neighbours)
            {
                if (closedList.Contains(neighbourPosition)) continue; // Skip if the neighbour is in the closed list
                
                Node neighbourNode = new Node(neighbourPosition);
                
                if (!isWalkableDuringAB(neighbourPosition)) continue; // Skip if the neighbour is not walkable
                
                int tentativeGCost = currentNode.gCost + 
                                     GetDistance(currentNode.position, neighbourPosition) +
                                     GetMovementCost(neighbourPosition);

                if (!openList.Contains(neighbourNode) || 
                    tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = GetDistance(neighbourPosition, target);
                    neighbourNode.parent = currentNode;
                    
                    //Debug.Log($"Neighbour: {neighbourPosition}, fCost: {neighbourNode.fCost}, gCost: {neighbourNode.gCost}, hCost: {neighbourNode.hCost}");
                    /*neighbourNode.fCost = neighbourNode.gCost + neighbourNode.hCost; // Update the fCost*/

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Enqueue(neighbourNode, neighbourNode.fCost); // Enqueue with fCost as the priority
                    }
                }
            }
        }
        
        Debug.Log("No path found");
        return null; // Return null if no path is found
    }
    
    private int GetDistance(Vector2Int positionA, Vector2Int positionB)
    {
        // Manhattan distance
        return Mathf.Abs(positionA.x - positionB.x) + Mathf.Abs(positionA.y - positionB.y);
    }
    
    private int GetMovementCost(Vector2Int position)
    {
        // Get the movement cost of the cell in the GridCell component
        return GameManager.instance.gridManager.grid[position.x, position.y].GetComponent<GridCell>().cost;
    }

    private List<Vector2Int> GetNeighbours(Vector2Int position)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(1, 0),   // Right
            new Vector2Int(-1, 0)   // Left
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbour = position + direction;
            if (IsWithinGrid(neighbour))
            {
                neighbours.Add(neighbour);
            }
        }
        
        return neighbours;
    }
    
    private bool IsWithinGrid(Vector2Int position)
    {
        return position.x >= 0 && position.x < GameManager.instance.gridManager.gridWidth &&
               position.y >= 0 && position.y < GameManager.instance.gridManager.gridHeight;
    }
    
    private bool isWalkableDuringAB(Vector2Int position)
    {
        return GameManager.instance.gridManager.grid[position.x, position.y].GetComponent<GridCell>().isWalkable;
    }
}
