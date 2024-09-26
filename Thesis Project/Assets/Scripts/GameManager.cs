using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Making the GameManager singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // For managers
    public GridManager gridManager;
    
    // For setting up the gird
    public int gridWidth = 5;
    public int gridHeight = 5;
    
    // For player
    private GameObject playerPrefab;
    private GameObject player;
    private GhostInteraction playerInteraction; // drag-n-drop before the game started
    private CharacterGhost characterGhost;      // character logic after the game started
    
    // For the shell
    private GameObject shellPrefab;
    private GameObject shell;
    
    // For start the battle
    public bool isBattleStarted = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // Wiring the managers
        gridManager = gameObject.GetComponent<GridManager>();
        
        // Make grid
        gridManager.CreateGrid(gridWidth, gridHeight);
        
        // PLAYER - GHOST
        // Getting the player prefab
        playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/Player");
        
        // Instantiate the player in the scene
        Vector3 playerPosition = new Vector3(
            gridManager.gridArray[Mathf.FloorToInt(gridWidth / 2), 0].transform.position.x, 
            0, 
            gridManager.gridArray[Mathf.FloorToInt(gridWidth / 2), 0].transform.position.z);
        player = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        
        // Wiring scripts
        characterGhost = player.GetComponent<CharacterGhost>();
        
        // SHELL
        // Getting the shell prefab
        shellPrefab = Resources.Load<GameObject>("Prefabs/Characters/Shell");
        
        // Instantiate the shell in the scene
        Vector3 shellPosition = new Vector3(
            gridManager.gridArray[Mathf.FloorToInt(gridWidth / 2), gridHeight - 2].transform.position.x,
            0,
            gridManager.gridArray[Mathf.FloorToInt(gridWidth / 2), gridHeight - 2].transform.position.z);
        shell = Instantiate(shellPrefab, shellPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // "Start" button got pressed, start the auto-battle
    public void StartBattle()
    {
        isBattleStarted = true;
        
        // Start player (ghost) movement
        characterGhost.MoveTo(new Vector2Int(Mathf.FloorToInt(gridWidth / 2), gridHeight - 2));
    }
    
    /// <summary>
    /// This method finds the path
    /// from the start position to the target position using A* pathfinding.
    /// </summary>
    public class Pathfinding
    {
        // Parameters:
        // - start: The starting grid position of the pawn
        // - target: The target grid position where the pawn needs to move
        // - grid: A boolean 2D array representing the walkable area of the grid
        //         (true means walkable, false means blocked)
        public List<Vector2Int> FindPath (Vector2Int start, Vector2Int target, bool[,] grid)
        {
            // This list stores the path nodes (grid positions) from start to target
            List<Vector2Int> path = new List<Vector2Int>();
            
            // Open list: keeps track of nodes that need to be checked / nodes to explore
            // Closed list: keeps track of nodes that have already been evaluated
            List<Vector2Int> openList = new List<Vector2Int>();
            HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();
            
            // Add the starting position to the open list
            openList.Add(start);
            
            // Loop to simulate pathfinding
            while (openList.Count > 0)
            {
                // Get the current node (the first one in the open list)
                Vector2Int currentNode = openList[0];
                
                openList.RemoveAt(0);  // Remove it from the open list
                closedList.Add(currentNode);  // Add it to the closed list
                
                // If the current node is the target, we've found a path
                if (currentNode == target)
                {
                    path.Add(currentNode);
                    break;
                }
                
                // Check neighboring nodes (up, down, left, right directions)
                foreach (Vector2Int neighbor in GetNeighbors(currentNode, grid))
                {
                    // If the neighbor hasn't been evaluated yet, add it to the open list
                    if (!closedList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
                
                // Add the current node to the path list
                path.Add(currentNode);
            }
            
            // Return the list of nodes that make up the path
            return path;
        }
        
        // This method gets the neighboring grid positions for a given node
        // Only up, down, left, and right neighbors are considered (no diagonals)
        private List<Vector2Int> GetNeighbors(Vector2Int node, bool[,] grid)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            
            
            // Check left (if it's within grid bounds)
            // Left
            if (node.x - 1 >= 0)
            {
                neighbors.Add(new Vector2Int(node.x - 1, node.y));
            }
            // Right
            if (node.x + 1 < grid.GetLength(0))
            {
                neighbors.Add(new Vector2Int(node.x + 1, node.y));
            }
            // Up
            if (node.y - 1 >= 0)
            {
                neighbors.Add(new Vector2Int(node.x, node.y - 1));
            }
            // Down
            if (node.y + 1 < grid.GetLength(1))
            {
                neighbors.Add(new Vector2Int(node.x, node.y + 1));
            }

            return neighbors;
        }
    }
}
