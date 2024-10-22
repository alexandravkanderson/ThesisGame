using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype_v_1_Scripts
{
    public enum CameraType
    {
        EnvironmentalLevel,
        AutoBattler
    }
    
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform; // Main camera transform
        [SerializeField] private Transform playerTransform; // Player transform
        
        [SerializeField] private CameraType cameraType;     // Camera type

        // CAMERA MOVEMENT
        [SerializeField] private float cameraMoveSpeed = 5f; // Camera movement speed
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 0f, -6f); // Camera offset from the player

        // CAMERA TRANSITION TO AUTOBATTLER
        [SerializeField] private Vector3 cameraPositionOffset = new Vector3(0f, 5f, 1f); // Camera position offset
        [SerializeField] private Vector3 cameraRotationOffset = new Vector3(45f, 0f, 0f); // Camera rotation offset
        [SerializeField] private float cameraTransitionDuration = 2f; // Camera transition duration

        private bool transitioningToAutoBattle = false; // Check if the camera is transitioning to the autobattler

        // Start is called before the first frame update
        void Start()
        {
            // Setting the camera transform
            cameraTransform = Camera.main.transform;
            
            // Initializing the camera type
            cameraType = CameraType.EnvironmentalLevel;

            // Setting the player transform
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (!transitioningToAutoBattle && 
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
                cameraMoveSpeed * Time.deltaTime);
        }
        
        public void StartCameraTransitionToAutobattler()
        {
            cameraType = CameraType.AutoBattler; // Set the camera type to autobattler
            transitioningToAutoBattle = true;

            StartCoroutine(CameraTransitionToAutobattler());
        }

        private IEnumerator CameraTransitionToAutobattler()
        {
            float elapsedTime = 0f; // Elapsed time

            // Get initial camera position and rotation
            Vector3 initialCameraPosition = cameraTransform.position;
            Quaternion initialCameraRotation = cameraTransform.rotation;

            Vector3 targetCameraPosition =
                initialCameraPosition + cameraPositionOffset; // Calculate target camera position
            Quaternion targetCameraRotation =
                Quaternion.Euler(cameraRotationOffset) * initialCameraRotation; // Calculate target camera rotation

            while (elapsedTime < cameraTransitionDuration)
            {
                elapsedTime += Time.deltaTime; // Increment elapsed time

                // Lerp the camera position and rotation
                cameraTransform.position = Vector3.Lerp(initialCameraPosition, targetCameraPosition,
                    elapsedTime / cameraTransitionDuration);
                cameraTransform.rotation = Quaternion.Lerp(initialCameraRotation, targetCameraRotation,
                    elapsedTime / cameraTransitionDuration);

                yield return null; // Wait for the next frame
            }

            // Set the camera position and rotation to the target position and rotation
            cameraTransform.position = targetCameraPosition;
            cameraTransform.rotation = targetCameraRotation;

            transitioningToAutoBattle = false; // Reset transitioning to autobattler to false
        }
    }

}