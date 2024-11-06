using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype_v_1_Scripts;

namespace Prototype_v_1_Scripts
{
    public class GridManager : MonoBehaviour
    {
        // GRID CELL
        public GameObject gridCellPrefab;
        public float gridCellSize = 1.0f;

        // GRID
        public int gridWidth;
        public int gridHeight;
        
        public GameObject[,] grid;
        private GameObject gridHolder;
        
        private Vector3 gridOrigin; // Origin of the grid, relative to the player
        
        public Material cellMaterialDefault;
        public Material cellMaterialOccupied;
        
        // PLAYER
        [SerializeField] private Transform playerTransform;
        
        // Default grid offset to place the player at certain position
        private Vector2Int defaultPlayerGridPosition = new Vector2Int(1, 2); // Third row, second column
        
        // Start is called before the first frame update
        void Start()
        {
            // Player
            playerTransform = PlayerController.instance.playerTransform;
            
            // Getting the grid cell prefab
            gridCellPrefab = Resources.Load<GameObject>("Prototype_v_1_Resources/Prefabs/Cell");
            
            // Create the holder for all cells
            gridHolder = new GameObject("Grid");
            gridHolder.transform.position = playerTransform.position; // Set the position to the origin
            
            // Materials
            cellMaterialDefault = Resources.Load<Material>("Prototype_v_1_Resources/Materials/CellDefault");
            cellMaterialOccupied = Resources.Load<Material>("Prototype_v_1_Resources/Materials/CellOccupied");
        }

        // Update is called once per frame
        void Update()
        {

        }

        // CREATE GRID, GRID HEIGHT, GRID WIDTH, PLAYER INITIAL GRID POSITION (IN 2D ARRAY, OPTIONAL)
        public void CreateGrid(int gridWidth, int gridHeight, 
                               int? playerGridPosX = null, int? playerGridPosY = null, float? groundLevelY = null)
        {
            // Set the grid width and height
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            
            // Set the default player position if not provided
            playerGridPosX ??= defaultPlayerGridPosition.x;
            playerGridPosY ??= defaultPlayerGridPosition.y;
            
            groundLevelY ??= 0.5f; // Set the ground level to 0 if not provided
            
            grid = new GameObject[gridWidth, gridHeight]; // Initialize the grid array
            
            // Calculate the world position offset based on player's position
            gridOrigin = GetGridOriginForPlayerPosition(playerGridPosX.Value, playerGridPosY.Value, groundLevelY.Value);
            
            // Create the grid
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    // Calculate the world position for each cell
                    Vector3 cellPosition = GetWorldPositionFromGridPosition(x, y); // Calculate the position for each cell (gridOrigin included)
                    
                    // Instantiate the cell, and assign the parent
                    GameObject newCellObject = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity) as GameObject; // Instantiate the cell
                    newCellObject.transform.parent = gridHolder.transform; // Assign the parent
                    newCellObject.name = $"Cell {x}, {y}"; // Name the cell for clarity in the hierarchy
                    
                    // Set the walkable / cost value of the cell
                    GridCell gridCell = newCellObject.GetComponent<GridCell>();
                    gridCell.isWalkable = true; // True by default
                    gridCell.cost = 1;          // Default cost
                    
                    grid[x, y] = newCellObject; // Store the cell in the grid array
                }
            }
            
            // Set the player's cell to occupied material
            SetGridCellMaterial(new Vector2Int(playerGridPosX.Value, playerGridPosY.Value), cellMaterialOccupied);
            playerTransform.GetComponent<PlayerController>().currentGridPosition = new Vector2Int(playerGridPosX.Value, playerGridPosY.Value);
        }
        
        private Vector3 GetGridOriginForPlayerPosition(int playerGridPosX, int playerGridPosY, float groundLevelY)
        {
            // Calculate the player's position in grid coordinates
            Vector2Int playerGridPosition = new Vector2Int(playerGridPosX, playerGridPosY);
            
            // Get the world position of the player's cell
            Vector3 playerGridWorldPosition = playerTransform.position -
                                              new Vector3(
                                                  playerGridPosition.x * gridCellSize, 
                                                  groundLevelY,
                                                  playerGridPosition.y * gridCellSize);
            
            return playerGridWorldPosition; // This is the origin of the grid relative to the player
        }
        
        public Vector3 GetWorldPositionFromGridPosition(int x, int y)
        {
            return gridOrigin + new Vector3(x * gridCellSize, 0, y * gridCellSize);
        }
        
        public Vector2Int GetGridPositionFromWorldPosition(Vector3 worldPosition)
        {
            // Subtract gridOrigin before converting to grid coordinates
            Vector3 offsetPosition = worldPosition - gridOrigin;
            return new Vector2Int(Mathf.FloorToInt(offsetPosition.x / gridCellSize), Mathf.FloorToInt(offsetPosition.z / gridCellSize));
        }

        // Check if the grid position is within the walkable area and is a cell walkable
        public bool IsWalkable(Vector2Int gridPosition)
        {
            bool isCellWalkable = grid[gridPosition.x, gridPosition.y].GetComponent<GridCell>().isWalkable;
            bool isWithinWalkableArea = gridPosition.x >= 0 && gridPosition.x < gridWidth / 2 && 
                                        gridPosition.y >= 0 && gridPosition.y < gridHeight;
            
            return isCellWalkable && isWithinWalkableArea;
        }

        public void SetGridCellMaterial(Vector2Int gridPosition, Material material)
        {
            if (grid[gridPosition.x, gridPosition.y] != null)
            {
                Renderer cellRenderer = grid[gridPosition.x, gridPosition.y].GetComponent<Renderer>();
                
                // Set the material
                if (cellRenderer != null)
                {
                    cellRenderer.material = material;
                }
            }
        }
    }
}
