using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype_v_1_Scripts;
using UnityEngine.Serialization;

namespace Prototype_v_1_Scripts
{
    public enum ControlType
    {
        EnvironmentalLevel,
        AutoBattler
    }

    public class PlayerController : MonoBehaviour
    {
        // SINGLETON
        public static PlayerController instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        
        // CAMERA
        private CameraManager cameraManager;
        
        // PLAYER COMPONENTS
        public Transform playerTransform;
        [SerializeField] private Rigidbody playerRigidbody;
        
        // PLAYER CONTROL TYPE
        public ControlType controlType;
        
        // WASD MOVEMENT -- ENVIRONMENTAL LEVEL
        [SerializeField] private float moveSpeedEL = 5f;  // Movement speed
        [SerializeField] private float maxSpeed = 10f;  // Maximum speed
        
        [SerializeField] private float jumpForce = 5f;  // Jump force
        [SerializeField] private AnimationCurve jumpCurve; 
        [SerializeField] private float jumpDuration = 0.5f;
        private float jumpTimer = 0f;
        
        private float fallSpeed = 0f;                   // Acceleration weight when falling
        [SerializeField] private float gravity = 10f;   // Gravity
        [SerializeField] private float gravityMultiplier = 2f;
        
        [SerializeField] private bool isGrounded;       // Check if the player is grounded
        
        [SerializeField] private bool isInteractable;    // Check if the player is available for interacting with an object
        
        // AUTOBATTLER MOVEMENT
        [SerializeField] private float moveSpeedAB = 10f;  // Movement speed
        [SerializeField] private AnimationCurve moveCurve; // Movement curve
        
        public Vector2Int currentGridPosition; // Current grid position
        
        [SerializeField] private bool isMoving = false; // Check if the player is moving across the grid
        
        // AUTOBATTLER DRAG N DROP
        private bool isDragging = false;

        // PATHFINDING
        private Vector2Int playerGridPosition;
        
        // Start is called before the first frame update
        void Start()
        {
            // Initializing the controller as environmental level
            controlType = ControlType.EnvironmentalLevel;
            
            // Camera
            cameraManager = GameManager.instance.gameObject.GetComponent<CameraManager>();
            
            // Initializing the player components
            playerTransform = transform;
            playerRigidbody = GetComponent<Rigidbody>();
            
            // Initializing the player's status
            isGrounded = true;
            jumpTimer = 0f;
        }

        void Update()
        {
            if (!cameraManager.transitionToAutoBattle &&
                controlType == ControlType.EnvironmentalLevel)
            {
                // JUMPING 
                if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                {
                    isGrounded = false; // The player is no longer grounded
                
                    jumpTimer = 0f; // Resetting the jump timer
                
                    fallSpeed = 0f; // Resetting the fall speed
                }
            
                // PICK UP INTERACTABLE OBJECT
                if (isInteractable)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.CompareTag("Interactable"))
                            {
                                // Pull the scriptable object from the interactable object
                                EnvironmentItemBase item = hit.transform.GetComponentInParent<EnvironmentItemBase>();
                                EnvironmentItemScriptableObject itemScriptableObject = item.environmentItemScriptableObject;
                                
                                // Add the object to the player's currency
                                GameManager.instance.shopManager.inventory.AddCurrency(itemScriptableObject.environmentItemName);                 // data
                                GameManager.instance.shopCurrencyUI.
                                    UpdateInventoryDisplay(itemScriptableObject.environmentItemName, itemScriptableObject.environmentItemSprite); // UI
                                
                                // Destroy the object
                                Destroy(hit.transform.parent.gameObject);
                            }
                        }
                    }
                }
            }
            
            /*if (!cameraManager.transitionToAutoBattle &&
                controlType == ControlType.AutoBattler)
            {
                if (!isMoving) // Check if the player is moving across the grid
                {
                    AutoBattlerController();
                }
            }*/
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // ENVIRONMENTAL LEVEL
            if (!cameraManager.transitionToAutoBattle &&
                controlType == ControlType.EnvironmentalLevel)
            {
                EnvironmentalLevelController();
            }
            // AUTOBATTLER
            else if (!cameraManager.transitionToAutoBattle &&
                     controlType == ControlType.AutoBattler)
            {
                if (!isMoving) // Check if the player is moving across the grid
                {
                    AutoBattlerController();
                }
            }
        }
        
        // ENVIRONMENTAL LEVEL MOVEMENT
        private void EnvironmentalLevelController()
        {
            // MOVEMENT
            // Getting the player's input
            float moveX = Input.GetAxis("Horizontal");

            // Check if there is any input
            if (Mathf.Abs(moveX) > 0.01f)
            {
                // Moving the player
                Vector3 movement = new Vector3(moveX, 0, 0) * (moveSpeedEL * Time.fixedDeltaTime);
            
                // Applying the movement
                playerRigidbody.velocity = movement;
            
                // Clamping the velocity to the maximum speed
                if (playerRigidbody.velocity.magnitude > maxSpeed)
                {
                    playerRigidbody.velocity = playerRigidbody.velocity.normalized * maxSpeed;
                }
            }
            else
            {
                // Stopping the player
                playerRigidbody.velocity = Vector3.zero;
            }
            
            //Debug.Log("Player's velocity: " + playerRigidbody.velocity.magnitude); // For debugging purposes
            
            // JUMPING
            if (!isGrounded)
            {
                playerRigidbody.useGravity = true;
                
                if (jumpTimer < jumpDuration)
                {
                    // Jumping with a curve
                    float jumpMultiplier = jumpCurve.Evaluate(jumpTimer / jumpDuration);
                    playerRigidbody.AddForce(Vector3.up * jumpForce * jumpMultiplier, ForceMode.Acceleration);
                    
                    jumpTimer += Time.fixedDeltaTime;
                }
                else
                {
                    // Increasing the fall speed
                    fallSpeed += gravityMultiplier * Time.fixedDeltaTime;
                    playerRigidbody.AddForce(Vector3.down * (gravity + fallSpeed), ForceMode.Acceleration);
                }
            }
            else
            {
                playerRigidbody.useGravity = false;
            }
        }
        
        private void OnCollisionEnter(Collision other)
        {
            // Check if the player is grounded
            if (other.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }
        
        private void OnCollisionStay(Collision other)
        {
            // Check if the player is grounded
            if (other.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            // Check if the player is grounded
            if (other.gameObject.CompareTag("Ground"))
            {
                isGrounded = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.gameObject.tag)
            {
                // Check if the player enters the autobattler trigger
                case "AutobattlerTrigger":
                    other.gameObject.SetActive(false); // Deactivate the trigger
                
                    playerRigidbody.velocity = Vector3.zero; // Stop the player's movement
                    GameManager.instance.CurrentProgression = GameProgression.Lv1HeartAutobattler; // Switch to autobattler
                    break;
                
                // Interactable object
                case "Interactable":
                    isInteractable = true;
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            switch (other.gameObject.tag)
            {
                case "Interactable":
                    isInteractable = false;
                    break;
            }
        }
        
        // AUTOBATTLER MOVEMENT
        private void AutoBattlerController()
        {
            // Stopping the player
            playerRigidbody.velocity = Vector3.zero;
            
            // Reset
            Vector3 moveDirection = Vector3.zero;
            
            // Getting the player's input
            if (Input.GetKey(KeyCode.W)) moveDirection += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) moveDirection += Vector3.back;
            if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
            if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;
            
            // Check if there is any input
            if (moveDirection != Vector3.zero)
            {
                Vector3 targetPosition = 
                    playerTransform.position + moveDirection.normalized * GameManager.instance.gridManager.gridCellSize;
                Vector2Int targetGridPosition = 
                    GameManager.instance.gridManager.GetGridPositionFromWorldPosition(targetPosition);

                if (GameManager.instance.gridManager.IsWalkable(targetGridPosition))
                {
                    // Move the player to the target position
                    StartCoroutine(MoveToPosition(targetPosition, targetGridPosition));
                }
            }
        }

        private IEnumerator MoveToPosition(Vector3 targetPosition, Vector2Int newGridPosition)
        {
            isMoving = true; // Set the player to be moving
            
            Vector3 startPosition = transform.position; // Get the player's current position
            
            float elapsedTime = 0f;
            float duration = 1f / moveSpeedAB; // Calculate the duration based on the movement speed

            while (elapsedTime < duration)
            {
                elapsedTime += Time.fixedDeltaTime; // Increment the elapsed time
                /*elapsedTime += Time.deltaTime; // Increment the elapsed time*/
                
                float t = elapsedTime / duration;   // Calculate the progress of the current time
                float curveT = moveCurve.Evaluate(t);  // Evaluate the movement curve
                
                // Lerp the player's position
                transform.position = Vector3.Lerp(startPosition, targetPosition, curveT); 
                yield return null; // Wait for the next frame
            }
            
            // Set the player's position to the target position after the movement is done
            transform.position = targetPosition; 
            
            // Set materials
            GameManager.instance.gridManager.SetGridCellMaterial(currentGridPosition, GameManager.instance.gridManager.cellMaterialDefault);
            GameManager.instance.gridManager.SetGridCellMaterial(newGridPosition, GameManager.instance.gridManager.cellMaterialOccupied);
            
            currentGridPosition = newGridPosition; // Update the current grid position
            
            isMoving = false; // Reset
        }
        
        // PATHFINDING MOVEMENT
        public void MoveToPosition(Vector3 targetPosition)
        {
            playerGridPosition = GameManager.instance.gridManager.GetGridPositionFromWorldPosition(playerTransform.position);
            Debug.Log("Player's grid position: " + playerGridPosition);
            
            Vector2Int targetGridPosition = GameManager.instance.gridManager.GetGridPositionFromWorldPosition(targetPosition);
            Debug.Log("Target grid position: " + targetGridPosition);
            
            // A star pathfinding
            List<Vector2Int> path = GameManager.instance.aStar.FindPath(playerGridPosition, targetGridPosition);
            
            if (path != null)
            {
                StartCoroutine(MoveAlongPath(path, targetGridPosition));
            }
        }

        private IEnumerator MoveAlongPath(List<Vector2Int> path, Vector2Int target)
        {
            // Iterate through each cell in the calculated path
            foreach (Vector2Int step in path)
            {
                // Check if the player is next to the target
                if (IsNextToTarget(step, target))
                {
                    // TODO ACTUAL ATTACK
                    InvokeRepeating(nameof(PlayerFakeAttack), 0f, 0.75f);
                    
                    break; // Stop moving if the player is next to the target
                }
                
                Vector3 targetWorldPosition =
                    GameManager.instance.gridManager.GetWorldPositionFromGridPosition(step.x, step.y);
                targetWorldPosition.y = playerTransform.position.y; // Keep the player's Y position

                // Move the player to the target position
                while (Vector3.Distance(transform.position, targetWorldPosition) > 0.1f)
                {
                    playerTransform.position =
                        Vector3.MoveTowards(
                            playerTransform.position,
                            targetWorldPosition,
                            moveSpeedAB * Time.deltaTime);

                    yield return null;
                }
                
                playerGridPosition = step; // Update the player's grid position
            }
        }
        
        private bool IsNextToTarget(Vector2Int current, Vector2Int target)
        {
            // Check if the pawn is next to the target in X or Z direction
            return (Mathf.Abs(current.x - target.x) == 0 && current.y == target.y) || 
                   (Mathf.Abs(current.y - target.y) == 0 && current.x == target.x);
        }

        // THIS IS A FAKE FUNCTION OF ATTACKING, PLEASE REPLACE THIS LATER
        private int counter = 0;

        public int Counter
        {
            get
            {
                return counter;
            }
            set
            {
                counter = value;
                if (counter == 10)
                {
                    CancelInvoke(nameof(PlayerFakeAttack));
                    counter = 0;
                    GameManager.instance.restartButton.SetActive(true);
                }
            }
        }
        
        private void PlayerFakeAttack()
        {
            Debug.Log("Player is attacking");
            Counter++;
            
            transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        }
        
        /*// TODO: CODE OF DRAGGING OBJECT, NEED TO BE FIXED
        private void MouseDrag()
        {
            // If player clicks on the player object
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == playerTransform)
                    {
                        Debug.Log("Player is clicked");
                        isDragging = true;
                    }
                }
            }
            
            // If player release the mouse button
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                SnapPlayerToGrid();
            }
            
            // If player is dragging the player object
            if (isDragging)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane plane = new Plane(Vector3.up, Vector3.zero); // Restrict movement to X-Z plane
                
                float distance;
                
                if (plane.Raycast(ray, out distance))
                {
                    Vector3 targetPosition = ray.GetPoint(distance);
                    targetPosition.y = playerTransform.position.y; // Keep the player's Y position
                    
                    playerTransform.position = targetPosition;
                }
            }
        }
        
        public void SnapPlayerToGrid()
        {
            Vector2Int gridPos = GameManager.instance.gridManager.GetGridPositionFromWorldPosition(playerTransform.position);
            Vector3 snappedPosition = GameManager.instance.gridManager.GetWorldPositionFromGridPosition(gridPos.x, gridPos.y);
            playerTransform.position = snappedPosition;
        }*/
    }
}
