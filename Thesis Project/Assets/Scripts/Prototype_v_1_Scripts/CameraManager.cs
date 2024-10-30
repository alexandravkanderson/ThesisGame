using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;

namespace Prototype_v_1_Scripts
{
    public enum CameraType
    {
        EnvironmentalLevel,
        AutoBattler
    }
    
    public class CameraManager : MonoBehaviour
    {
        // TRANSFORMS
        [SerializeField] private Transform cameraTransform; // Main camera transform
        [SerializeField] private Transform playerTransform; // Player transform
        [SerializeField] private Transform shellTransform;  // Shell transform
        
        [SerializeField] private CameraType cameraType;     // Camera type

        // CAMERA MOVEMENT
        [SerializeField] private float cameraMoveSpeed = 5f; // Camera movement speed
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 0f, -6f); // Camera offset from the player

        // CAMERA TRANSITION TO AUTOBATTLER
        [SerializeField] private Vector3 cameraPositionTransitionOffset = new Vector3(0f, 5f, 1f); // Camera position offset
        [SerializeField] private Vector3 cameraRotationTransitionOffset = new Vector3(45f, 0f, 0f); // Camera rotation offset
        [SerializeField] private float cameraPositionTransitionDuration = 2f; // Camera POSITION transition duration
        [SerializeField] private float cameraRotationTransitionDuration = 2f; // Camera ROTATION transition duration
        [SerializeField] private float playerRotationMultiplier = 0.5f; // Player rotation multiplier

        public bool transitionToAutoBattle = false; // Check if the camera is transition-ing to the autobattler
        
        // YARN SPINNER
        private DialogueRunner dialogueRunner;

        // Start is called before the first frame update
        void Start()
        {
            // Setting the camera transform
            cameraTransform = Camera.main.transform;
            
            // Initializing the camera type
            cameraType = CameraType.EnvironmentalLevel;

            // Setting the player && shell transform
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            shellTransform = GameObject.FindGameObjectWithTag("Shell").transform;
            
            // Setting the dialogue runner
            dialogueRunner = GameManager.instance.dialogueRunner;
            
            // TEST DEBUG AREA, DONOT TOUCH
            //dialogueRunner.AddCommandHandler("StartCameraTransitionToAutobattlerRotation", StartCameraTransitionToAutobattlerRotation);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!transitionToAutoBattle && 
                cameraType == CameraType.EnvironmentalLevel)
            {
                CameraFollowingPlayer();
            }
        }

        private void CameraFollowingPlayer()
        {
            // Calculate the target camera position
            Vector3 targetCameraPosition = playerTransform.position + cameraOffset;

            // Lerp the camera position
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetCameraPosition,
                cameraMoveSpeed * Time.fixedDeltaTime);
        }
        
        public void StartCameraTransitionToAutobattlerPosition([CanBeNull] string dialogueNode)
        {
            cameraType = CameraType.AutoBattler; // Set the camera type to autobattler
            transitionToAutoBattle = true;

            StartCoroutine(CameraTransitionToPosition(dialogueNode));
        }

        // CAMERA TRANSITION TO THE MIDPOINT POSITION
        private IEnumerator CameraTransitionToPosition([CanBeNull] string dialogueNode)
        {
            float elapsedTime = 0f; // Elapsed time
            
            // Get initial camera position
            Vector3 initialCameraPosition = cameraTransform.position;
            
            // Calculate the target position: the midpoint position between the player and the shell
            Vector3 midpointPosition = (playerTransform.position + shellTransform.position) / 2f;
            Vector3 targetCameraPosition = midpointPosition + cameraOffset; // Calculate target camera position

            while (elapsedTime < cameraPositionTransitionDuration)
            {
                elapsedTime += Time.fixedDeltaTime; // Increment elapsed time
                
                // Lerp the camera position 
                cameraTransform.position = Vector3.Lerp(initialCameraPosition, targetCameraPosition,
                    elapsedTime / cameraPositionTransitionDuration);
                
                yield return null; // Wait for the next frame
            }
            
            // Set the camera position to the target position
            cameraTransform.position = targetCameraPosition;
            
            RunDialogue(dialogueNode); // Run the dialogue
        }
        
        private void RunDialogue([CanBeNull] string dialogueNode)
        {
            // If need play the dialogue before the camera rotates
            if (dialogueNode != null)
            {
                dialogueRunner.StartDialogue(dialogueNode);
            }
            // If not, rotate the camera to the autobattler rotation
            else
            {
                StartCameraTransitionToAutobattlerRotation();
            }
        }
        
        // CAMERA TRANSITION TO THE AUTOBATTLE ROTATION
        [YarnCommand("StartCameraTransitionToAutobattlerRotation")]
        public void StartCameraTransitionToAutobattlerRotation()
        {
            // Rotate the camera to the autobattler rotation
            StartCoroutine(CameraTransitionToRotation());
            
            // TODO: TEMPORARY, GENERATE THE GRIDS
            GameManager.instance.gridManager.CreateGrid(6, 5);
            
            // TODO: PULL UP THE SHOP UI
            GameManager.instance.shopManager.ToggleDrawer();
        }
        
        // CAMERA TRANSITION TO THE AUTOBATTLE ROTATION
        private IEnumerator CameraTransitionToRotation()
        {
            float elapsedTime = 0f; // Elapsed time
            
            // Get initial camera position and rotation
            Vector3 initialCameraPosition = cameraTransform.position;
            Quaternion initialCameraRotation = cameraTransform.rotation;
            
            Vector3 targetCameraPosition =
                initialCameraPosition + cameraPositionTransitionOffset; // Calculate target camera position
            Quaternion targetCameraRotation =
                Quaternion.Euler(cameraRotationTransitionOffset) * initialCameraRotation; // Calculate target camera rotation

            while (elapsedTime < cameraRotationTransitionDuration)
            {
                elapsedTime += Time.fixedDeltaTime; // Increment elapsed time
                
                // Lerp the camera position and rotation
                cameraTransform.position = Vector3.Lerp(initialCameraPosition, targetCameraPosition,
                    elapsedTime / cameraRotationTransitionDuration);
                cameraTransform.rotation = Quaternion.Lerp(initialCameraRotation, targetCameraRotation,
                    elapsedTime / cameraRotationTransitionDuration);
                
                // Lerp the player && shell rotation to the camera rotation
                playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, targetCameraRotation,
                    elapsedTime / cameraRotationTransitionDuration * playerRotationMultiplier);
                shellTransform.rotation = Quaternion.Lerp(shellTransform.rotation, targetCameraRotation,
                    elapsedTime / cameraRotationTransitionDuration * playerRotationMultiplier);

                yield return null; // Wait for the next frame
            }
            
            // Set the camera position and rotation to the target position and rotation
            cameraTransform.position = targetCameraPosition;
            cameraTransform.rotation = targetCameraRotation;
            
            // Set the player rotation to the target rotation
            playerTransform.rotation = targetCameraRotation;

            transitionToAutoBattle = false; // Reset transitioning to autobattler to false
        }
    }

}