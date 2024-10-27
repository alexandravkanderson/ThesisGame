using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype_v_1_Scripts;

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
        
        // Camera
        private CameraManager cameraManager;
        
        // PLAYER COMPONENTS
        public Transform playerTransform;
        [SerializeField] private Rigidbody playerRigidbody;
        
        // PLAYER CONTROL TYPE
        public ControlType controlType;
        
        // WASD MOVEMENT -- ENVIRONMENTAL LEVEL
        [SerializeField] private float moveSpeed = 5f;  // Movement speed
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
        
        
        private bool isDragging = false;

        // Start is called before the first frame update
        void Start()
        {
            // Initializing the controller as environmental level
            controlType = ControlType.EnvironmentalLevel;
            
            // Camera
            cameraManager = GameManager.instance.gameObject.GetComponent<CameraManager>();
            
            // Initializing the player components
            playerTransform = GetComponent<Transform>();
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
            
                // INTERACTABLE OBJECT
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
                                Debug.Log("Interactable object is clicked");
                                Destroy(hit.transform.parent.gameObject);
                            }
                        }
                    }
                }
            }
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
                AutoBattlerController();
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
                Vector3 movement = new Vector3(moveX, 0, 0) * (moveSpeed * Time.fixedDeltaTime);
            
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
            
            
        }
        
        // TODO: FIX DRAGGING OBJECT
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
        }
    }
}
