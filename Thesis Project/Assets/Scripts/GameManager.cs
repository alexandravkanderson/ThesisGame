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
    private GridManager gridManager;
    
    // For setting up the gird
    public int gridWidth = 5;
    public int gridHeight = 5;
    
    // For player
    private GameObject playerPrefab;
    private GameObject player;
    private GhostInteraction playerInteraction;
    
    // For the shell
    private GameObject shellPrefab;
    private GameObject shell;
    
    // For start the battle
    [SerializeField] private bool isBattleStarted = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // Wiring the managers
        gridManager = gameObject.GetComponent<GridManager>();
        
        // Make grid
        gridManager.CreateGrid(gridWidth, gridHeight);
        
        // Getting the player prefab
        playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/Player");
        
        // Instantiate the player in the scene
        Vector3 playerPosition = new Vector3(
            gridManager.girdArray[Mathf.FloorToInt(gridWidth / 2), 0].transform.position.x, 
            0, 
            gridManager.girdArray[Mathf.FloorToInt(gridWidth / 2), 0].transform.position.z);
        player = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // "Start" button got pressed, start the auto-battle
    public void StartBattle()
    {
        isBattleStarted = true;
    }
    
    // For the Ghost pathfinding
    Vector3 GetWorldPosition(int x, int y)
    {
        return Vector3.zero;
    }
}
