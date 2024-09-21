using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Making the GameManager singleton
    public static GameManager instanse;

    private void Awake()
    {
        if (instanse == null)
        {
            instanse = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // For managers
    [SerializeField] private GridManager gridManager;
    
    // For setting up the gird
    public int gridWidth = 4;
    public int gridHeight = 4;
    
    // Start is called before the first frame update
    void Start()
    {
        // Wiring the managers
        gridManager = gameObject.GetComponent<GridManager>();
        
        // Make grid
        gridManager.CreateGrid(gridWidth, gridHeight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
