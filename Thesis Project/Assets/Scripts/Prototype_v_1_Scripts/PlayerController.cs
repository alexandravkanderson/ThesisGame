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
        
        // PLAYER COMPONENTS
        [SerializeField] private Transform playerTransform;
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

        // Start is called before the first frame update
        void Start()
        {
            // Initializing the controller as environmental level
            controlType = ControlType.EnvironmentalLevel;
            
            // Initializing the player components
            playerTransform = GetComponent<Transform>();
            playerRigidbody = GetComponent<Rigidbody>();
            
            // Initializing the player's status
            isGrounded = true;
            jumpTimer = 0f;
        }

        void Update()
        {
            // JUMPING 
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                isGrounded = false; // The player is no longer grounded
                
                jumpTimer = 0f; // Resetting the jump timer
                
                fallSpeed = 0f; // Resetting the fall speed
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // ENVIRONMENTAL LEVEL
            if (controlType == ControlType.EnvironmentalLevel)
            {
                EnvironmentalLevelController();
            }
            // AUTOBATTLER
            else if (controlType == ControlType.AutoBattler)
            {
                
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
            
            Debug.Log("Player's velocity: " + playerRigidbody.velocity.magnitude); // For debugging purposes
            
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
            // Check if the player enters the autobattler trigger
            if (other.gameObject.CompareTag("AutobattlerTrigger"))
            {
                playerRigidbody.velocity = Vector3.zero; // Stop the player's movement
                GameManager.instance.CurrentProgression = GameProgression.Lv1HeartAutobattler;
            }
        }
    }
}
