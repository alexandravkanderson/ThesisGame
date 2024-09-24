using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    /// <summary>
    /// This script is the .base class
    /// of the both ghost-drag-n-drop and shop-drag-n-drop interaction 
    /// </summary>
    
    private Camera mainCamera;
    public GameObject selectedObject;
    [SerializeField] private bool isDragging;

    protected string tagName = "";

    private Vector3 dragOffset;
    public Vector3 originalPosition;  // Store the original position
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Get the main camera
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // If it's before the battle started, interact
        // otherwise, forbid interaction
        if (!GameManager.instance.isBattleStarted)
        {
            // CLICK
            // If the left mouse button is down
            if (Input.GetMouseButtonDown(0))
            {
                // Cast a ray, from the camera to the mouse position
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Check if the ray hits any object
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the object is interactable
                    if (hit.collider.CompareTag(tagName)) // tagName override in the sub-class
                    {
                        // Store the selected object (parent)
                        selectedObject = hit.collider.gameObject.transform.parent.gameObject;
                        isDragging = true;
                    
                        // For debugging only
                        Debug.Log(selectedObject.name);
                    
                        // Store original position
                        originalPosition = selectedObject.transform.position;
                    
                        // Calculate the offset between the object and the point where the ray hit it
                        dragOffset = selectedObject.transform.position - hit.point;
                    }
                }
            }
        
            // DRAG
            // While the mouse button is held down, move the object
            if (Input.GetMouseButton(0) 
                && isDragging 
                && selectedObject != null)
            {
                // Move the selected object to follow the mouse
                DraggingObject();
            }
        
            // DROP
            // If the mouse button is released, stop dragging
            if (Input.GetMouseButtonUp(0) 
                && isDragging)
            {
                isDragging = false;
            
                // Things happens after dropping the object
                ReleasingObject();
            }
        }
    }

    // Moves the selected object to the mouse position
    protected virtual void DraggingObject()
    {
        // Cast a ray, from the camera to the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Plane for object movement along X-Z
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        float distance;

        // Check for ray--plane intersection
        if (plane.Raycast(ray, out distance))
        {
            // Calculate the intersection point
            Vector3 point = ray.GetPoint(distance);
            
            // Move the object to the intersection point
            selectedObject.transform.position = new Vector3(
                point.x + dragOffset.x, 
                selectedObject.transform.position.y, 
                point.z + dragOffset.z);
        }
    }
    
    protected virtual void ReleasingObject()
    {
        // Override in the sub-class
        
        // NOTE:
        // GhostInteraction -- Snapping the pawn to the grid
        // Shop -- 
    }
}
