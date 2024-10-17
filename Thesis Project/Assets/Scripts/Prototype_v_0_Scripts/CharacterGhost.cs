using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Prototype_v_0_Scripts;

namespace Prototype_v_0_Scripts
{
    public class CharacterGhost : Characters
    {
        public GridManager gridManager;
    
        // For pathfinding system (A* algorithm)
        public GameManager.Pathfinding pathfinding;
        private Vector2Int selfGridPosition;
    
        public float moveSpeed = 2.0f;

        // For player attacking the enemy
        [SerializeField] private bool isAttacking = false;
        [SerializeField] private ParticleSystem attactParticleSystem;

        // The volume for post-processing
        [SerializeField] private Volume globalVolume;
        [SerializeField] private Vignette vignette;
    
        private CharacterShell characterShell;

        public float playerHP
        {
            get
            {
                return base.HP;
            }
            set
            {
                base.HP = value;

                if (base.HP <=0 )
                {
                    // If the player's HP is 0, end the game,
                    // player loses
                    GameManager.instance.isPlayerWin = false;
                    GameManager.instance.GameEnding();
                    Destroy(gameObject);
                }
                else
                {
                    if (base.HP <= 5)
                    {
                        // Linking the HP to post processing
                        vignette.intensity.value = Mathf.Clamp(vignette.intensity.value + 0.1f, 0, 1);
                    }
                
                    Debug.Log("Player HP: " + base.HP);
                }
            }
        }
    
        protected override void Start()
        {
            base.Start();
        
            // Wiring
            gridManager = GameManager.instance.gridManager;
        
            globalVolume = GameObject.Find("Global Volume").GetComponent<Volume>();
            if (globalVolume.profile.TryGet(out vignette))
            {
                // Set default 
                vignette.intensity.value = 0.2f;
            }

            // Player (Ghost) attributes
            playerHP = base.HP;
        
            // Enemy (Shell) attributes
            characterShell = GameManager.instance.characterShell;
        
            // VFX child & particle system
            attactParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        // Move the pawn to the grid position
        public void MoveTo(Vector2Int targetGridPosition)
        {
            Debug.Log(targetGridPosition);
        
            // Get the grid position of self
            selfGridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);
        
            // Calculate the path from the current position to the target grid position
            List<Vector2Int> path = new GameManager.Pathfinding().FindPath(
                selfGridPosition, 
                targetGridPosition, 
                gridManager.walkableGridArray);
        
            // If a valid path is found, start a coroutine
            // to move the pawn along the path step by step
            if (path != null && path.Count > 0)
            {
                StartCoroutine(MoveAlongPath(path, targetGridPosition));
            }
        }

        // Coroutine to move the pawn along the path, one grid cell at a time
        private IEnumerator MoveAlongPath(List<Vector2Int> path, Vector2Int target)
        {
            // Iterate through each cell in the calculated path
            foreach (Vector2Int cell in path)
            {
                // Check if the ghost pawn is next to the shell
                if (IsNextToTarget(cell, target))
                {
                    // Shell starts to attack the player
                    characterShell.ShellAttack();
                    // Player starts to attack the shell
                    PlayerAttack();
                
                    break; // Stop moving if next to the target grid
                }
            
                // Get world position
                Vector3 targetPosition = gridManager.GetWorldPositionFromGridPosition(cell.x, cell.y);

                targetPosition.y = transform.position.y;
            
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
                selfGridPosition = cell;
            }
        }

        private bool IsNextToTarget(Vector2Int current, Vector2Int target)
        {
            // Check if the pawn is next to the target in X or Z direction
            return (Mathf.Abs(current.x - target.x) == 0 && current.y == target.y) || 
                   (Mathf.Abs(current.y - target.y) == 0 && current.x == target.x);
        }
    
        // Attack
        private void PlayerAttack()
        {
            isAttacking = true;
        
            InvokeRepeating(nameof(PlayerDealDamage), base.SP, base.SP);
        }

        private void PlayerDealDamage()
        {
            // If the game isn't ended, dell "AD" amount of damage
            if (!GameManager.instance.isGameEnd)
            {
                characterShell.ShellHP -= AD;
            
                // VFX animation
                attactParticleSystem.Play();
            }
        }
    }
}
