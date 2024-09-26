using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGhost : Characters
{
    public GridManager gridManager;
    
    // For pathfinding system (A* algorithm)
    public GameManager.Pathfinding pathfinding;
    private Vector2Int gridPosition;
    
    public float moveSpeed = 1.0f;
    
    protected override void Start()
    {
        base.Start();
        
        // Wiring
        gridManager = GameManager.instance.gridManager;

        // Get the grid position of self
        gridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);
    }

    // Move the pawn to the grid position
    public void MoveTo(Vector2Int targetGridPosition)
    {
        // Calculate the path from the current position to the target grid position
        List<Vector2Int> path = new GameManager.Pathfinding().FindPath(
            gridPosition, 
            targetGridPosition, 
            gridManager.walkableGridArray);
        
        // If a valid path is found, start a coroutine
        // to move the pawn along the path step by step
        if (path != null && path.Count > 0)
        {
            StartCoroutine(MoveAlongPath(path));
        }
    }

    // Coroutine to move the pawn along the path, one grid cell at a time
    private IEnumerator MoveAlongPath(List<Vector2Int> path)
    {
        // Iterate through each cell in the calculated path
        foreach (Vector2Int cell in path)
        {
            // Get world position
            Vector3 targetPosition = gridManager.GetWorldPositionFromGridPosition(cell.x, cell.y);
            
            // Move the pawn towards the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, 
                    targetPosition, 
                    moveSpeed * Time.deltaTime);
                
                yield return null; // Wait, until the next frame
            }
            
            // Update the pawn's cell position
            gridPosition = cell;
        }
    }
}
