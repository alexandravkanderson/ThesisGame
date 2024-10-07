using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public CharacterGhost characterGhost;       // character logic after the game started
    
    // For the shell
    private GameObject shellPrefab;
    private Vector3 shellPosition;
    private GameObject shell;
    public CharacterShell characterShell;       // character logic after the game started
    
    // For start the battle
    public bool isBattleStarted = false;
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    
    // For the battle ending
    public bool isGameEnd = false;
    public bool isPlayerWin = true;
    
    // For Shop
    [SerializeField] private GameObject spawnedItem;
    [SerializeField] private GameObject shopUI;
    
    // Start is called before the first frame update
    void Start()
    {
        // Wiring the managers
        gridManager = gameObject.GetComponent<GridManager>();
        
        // Wiring UI elements
        shopUI = GameObject.Find("Canvas");
        
        startButton = shopUI.transform.GetChild(0).GetComponent<Button>();
        startButton.gameObject.SetActive(true);

        restartButton = shopUI.transform.GetChild(1).GetComponent<Button>();
        restartButton.gameObject.SetActive(false);
        
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
        shellPosition = new Vector3(
            gridManager.gridArray[Mathf.FloorToInt(gridWidth / 2), gridHeight - 2].transform.position.x,
            0,
            gridManager.gridArray[Mathf.FloorToInt(gridWidth / 2), gridHeight - 2].transform.position.z);
        shell = Instantiate(shellPrefab, shellPosition, Quaternion.identity);
        
        // Wiring scripts
        characterShell = shell.GetComponent<CharacterShell>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // "Start" button got pressed, start the auto-battle
    public void StartBattle()
    {
        isBattleStarted = true;
      
        // Hide the Start button
        startButton.gameObject.SetActive(false);
        
        // Start player (ghost) movement
        characterGhost.MoveTo(gridManager.GetGridPositionFromWorldPosition(shellPosition)); // new Vector2Int(Mathf.FloorToInt(gridWidth / 2), gridHeight - 2)
    }
    
    /// <summary>
    /// This method finds the path
    /// from the start position to the target position using A* pathfinding.
    /// </summary>
    
    // Node contains the grid position, walk-ability, and cost values
    // The "parent" is used to retrace the path once the target is reached
    public class Node
    {
        public Vector2Int Position;   // The grid position
        public bool IsWalkable;       // Whether this node is walkable
        public int gCost;             // Cost from start to this node
        public int hCost;             // Heuristic cost from this node to the target
        public Node Parent;           // Parent node for path retracing

        public int fCost => gCost + hCost;  // Total cost

        public Node(Vector2Int position, bool isWalkable)
        {
            Position = position;
            IsWalkable = isWalkable;
        }
    }
    
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
            
            // Create a 2D array of nodes for the grid
            Node[,] nodeGrid = new Node[grid.GetLength(0), grid.GetLength(1)];

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    nodeGrid[x, y] = new Node(new Vector2Int(x, y), grid[x, y]);
                }
            }
            
            // Open list: keeps track of nodes that need to be checked / nodes to explore
            // Closed list: keeps track of nodes that have already been evaluated
            List<Node> openList = new List<Node>();
            HashSet<Node> closedList = new HashSet<Node>();

            Node startNode = nodeGrid[start.x, start.y];
            Node targetNode = nodeGrid[target.x, target.y];
            
            // Add the starting position to the open list
            openList.Add(startNode);
            
            // Loop to simulate pathfinding
            // This loop runs until there are no nodes left in the open list
            while (openList.Count > 0)
            {
                // Get the current node (the first one in the open list)
                Node currentNode = openList[0];

                // The current node is selected based on the lowest fCost.
                // If two nodes have the same fCost,
                // the one with the lower hCost is chosen (favoring those closer to the target)
                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].fCost < currentNode.fCost ||
                        openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
                    {
                        currentNode = openList[i];
                    }
                }
                
                openList.Remove(currentNode);  // Remove it from the open list
                closedList.Add(currentNode);   // Add it to the closed list
                
                // If the current node is the target, we've found a path
                if (currentNode.Position == targetNode.Position)
                {
                    path = RetracePath(startNode, currentNode);
                    break;
                }
                
                // Check neighboring nodes (up, down, left, right directions)
                foreach (Node neighbor in GetNeighbors(currentNode, nodeGrid))
                {
                    // If the neighbor is not walkable
                    // or has already been evaluated (in the closed list), it is skipped.
                    if (!neighbor.IsWalkable ||
                        closedList.Contains(neighbor))
                    {
                        continue;
                    }
                    
                    // The movement cost to the neighbor is calculated
                    int newMovementCostToNeighbor = currentNode.gCost + 10; // Cost to move to neighbor
                    
                    // If it's lower than the neighbor's current gCost,
                    // the neighbor's costs are updated
                    // and its parent is set to the current node
                    if (newMovementCostToNeighbor < neighbor.gCost ||
                        !openList.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetManhattanDistance(neighbor.Position, targetNode.Position);
                        neighbor.Parent = currentNode;

                        // If the neighbor is not already in the open list, it is added
                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }
            
            foreach (Vector2Int node in path)
            {
                Debug.Log($"Path node: {node}");
            }
            
            // Return the list of nodes that make up the path
            return path;
        }

        private int GetManhattanDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
        
        // Once the target is reached,
        // this method retraces the path from the end node back to the start node
        // using the parent references
        private List<Vector2Int> RetracePath(Node startNode, Node endNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }
            
            path.Reverse();
            return path;
        }
        
        // This method gets the neighboring grid positions for a given node
        // Only up, down, left, and right neighbors are considered (no diagonals)
        private List<Node> GetNeighbors(Node node, Node[,] grid)
        {
            List<Node> neighbors = new List<Node>();
            
            // Check the four adjacent nodes
            int x = node.Position.x;
            int y = node.Position.y;
            
            // Left
            if (x - 1 >= 0)
            {
                neighbors.Add(grid[x - 1, y]);
            }
            // Right
            if (x + 1 < grid.GetLength(0))
            {
                neighbors.Add(grid[x + 1, y]);
            }
            // Up
            if (y - 1 >= 0)
            {
                neighbors.Add(grid[x, y - 1]);
            }
            // Down
            if (y + 1 < grid.GetLength(1))
            {
                neighbors.Add(grid[x, y + 1]);
            }

            return neighbors;
        }
    }

    public void GameEnding()
    {
        if (!isGameEnd)
        {
            if (isPlayerWin)
            {
                Debug.Log("The Player Wins.");
            }
            else
            {
                Debug.Log("The Player Loses.");
            }

            isGameEnd = true;
            
            // Show restart
            restartButton.gameObject.SetActive(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Start_Screen");
    }
}
