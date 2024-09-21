using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Create a var for holding the grid cell
    public GameObject gridCellPrefab;
    
    // Create a 2D-array to generate the grid
    public GameObject[,] girdArray;
    private GameObject cellParent;
    
    // Start is called before the first frame update
    void Start()
    {
        // Getting the grid cell prefab
        gridCellPrefab = Resources.Load<GameObject>("Prefabs/GridCell/Cell");
        
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
        girdArray = new GameObject[gridWidth, gridHeight];
        
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
                girdArray[x, y] = newCell;
            }
        }
        
    }

    private Vector3 GetWorldPositionFromGridPosition(int x, int z)
    {
        return new Vector3(
            x - GameManager.instanse.gridWidth/2 + 0.5f, 
            0.5f, 
            z - GameManager.instanse.gridHeight/2 + 0.5f);
    }
}
