using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostInteraction : PlayerInteraction
{
    private GridManager gridManager;
    
    protected override void Start()
    {
        base.Start();

        // If the ray hit the object with tag "Player"
        // determine it as interactable
        tagName = "Player";
        
        // Get gridManager
        gridManager = GameManager.instance.gameObject.GetComponent<GridManager>();
    }

    protected override void ReleasingObject()
    {
        base.ReleasingObject();

        if (selectedObject != null)
        {
            // If the dropping position is inside the boundaries of the grid
            if (IsInGridArea(selectedObject.transform.position))
            {
                // Snap the pawn to the grid
                SnapToGrid(selectedObject);
            }
            else
            {
                // Snap the pawn back to original place
                selectedObject.transform.position = originalPosition;
            }
            
            selectedObject = null;
        }
    }

    protected virtual void SnapToGrid(GameObject pawn)
    {
        Vector3 position = pawn.transform.position;
        
        // Round the position to the nearest grid cell
        float x = Mathf.Round(position.x);
        float z = Mathf.Round(position.z) - 0.5f;
        
        // Snap the object to the nearest grid cell
        pawn.transform.position = new Vector3(x, pawn.transform.position.y, z);
    }

    protected virtual bool IsInGridArea(Vector3 pos)
    {
        // Calculating the boundaries of the grid
        // NOTE: fragile, DO NOT touch the offset numbers
        return pos.x >= gridManager.girdArray[0, 0].transform.position.x - 0.5f &&
               pos.x <= gridManager.girdArray[GameManager.instance.gridWidth - 1, 0].transform.position.x + 0.5f &&
               pos.z >= gridManager.girdArray[0, 0].transform.position.z &&
               pos.z <= gridManager.girdArray[0, GameManager.instance.gridHeight/2 - 1].transform.position.z + 0.5f;
    }
}
