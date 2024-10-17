using UnityEngine;
using Prototype_v_0_Scripts;

namespace Prototype_v_0_Scripts
{
    public class GridManager : MonoBehaviour
    {
        // Create a var for holding the grid cell
        public GameObject gridCellPrefab;
    
        // Create a 2D-array to generate the grid
        public GameObject[,] gridArray;
        private GameObject cellParent;
    
        // A 2D array to store walkable information
        public bool[,] walkableGridArray;
    
        // Start is called before the first frame update
        void Start()
        {
            // Getting the grid cell prefab
            gridCellPrefab = Resources.Load<GameObject>("Prototype_v_0_Resources/Prefabs/GridCell/Cell");
        
            // Create the holder for all cells
            cellParent = new GameObject("Grid");
            cellParent.transform.position = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void CreateGrid(int gridWidth, int gridHeight)
        {
            // Initialize the grid array
            gridArray = new GameObject[gridWidth, gridHeight];
            walkableGridArray = new bool[gridWidth, gridHeight];
        
            // Create the grid
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    // Calculate the position for each cell
                    Vector3 cellPosition = GetWorldPositionFromGridPosition(x, y);
                
                    // Instantiate the cell, and assign the parent
                    GameObject newCell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity) as GameObject;
                    newCell.transform.parent = cellParent.transform;
                
                    // Name the cell for clarity in the hierarchy
                    newCell.name = $"Cell {x}, {y}";
                
                    // Store the cell in the grid array
                    gridArray[x, y] = newCell;

                    // Set the cell as walkable
                    walkableGridArray[x, y] = true;
                
                    // If this grid is in the front half,
                    // set it to be ghost's grid
                    if (y < gridHeight / 2)
                    {
                        // Color-tint
                        gridArray[x, y].GetComponent<MeshRenderer>().material.color = 
                            Color.HSVToRGB(0f, 0f, 0.6f);
                    }
                }
            }
        
        }

        public Vector3 GetWorldPositionFromGridPosition(int x, int z)
        {
            return new Vector3(
                x - GameManager.instance.gridWidth / 2, 
                - 0.5f, 
                z - GameManager.instance.gridHeight / 2 + 0.5f);
        }

        public Vector2Int GetGridPositionFromWorldPosition(Vector3 worldPosition)
        {
            int x = Mathf.RoundToInt(worldPosition.x + GameManager.instance.gridWidth / 2);
            int y = Mathf.RoundToInt(worldPosition.z + GameManager.instance.gridHeight / 2 - 0.5f);

            return new Vector2Int(x, y);
        }
    }
}
