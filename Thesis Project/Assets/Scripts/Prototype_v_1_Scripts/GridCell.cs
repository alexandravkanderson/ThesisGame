using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public bool isWalkable;
    public int cost;
    
    private void Start()
    {
        isWalkable = true;
        cost = 1;
    }
    
    public void SetGridCell(bool walkable, int? c)
    {
        isWalkable = walkable;
        cost = c ?? 1; // Default cost is 1
    }
}
