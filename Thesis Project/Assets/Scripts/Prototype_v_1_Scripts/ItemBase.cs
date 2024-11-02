using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Prototype_v_1_Scripts;

namespace Prototype_v_1_Scripts
{
    public class ItemBase : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        // VARIABLES
        [SerializeField] private ShopItemScriptableObject shopItemScriptableObject;
        [SerializeField] private GameObject shopItemPrefab;

        public float zOffset;

        // DRAG AND DROP
        private GameObject selectedItem;
        private Vector3 originalPosition;
        
        [SerializeField] private bool isDragging;
        private Vector3 dragOffset;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            // If the control type is autobattler, activate the shop interaction
            if (PlayerController.instance.controlType == ControlType.AutoBattler)
            {
                ItemDragAndDrop();
            }
        }

        // BUTTON DOWN: INSTANTIATE THE ITEM
        public void OnPointerDown(PointerEventData eventData)
        {
            if (GameManager.instance.shopManager.isDrawerExpanded)
            {
                Debug.Log("Pointer down");
                
                // Get the mouse position in game world
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = zOffset;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                
                // Instantiate the item
                selectedItem = Instantiate(shopItemPrefab, worldPosition, Quaternion.Euler(GameManager.instance.cameraRotation));
                
                // Enable dragging for the instantiated item
                isDragging = true;
                
                // Store the initial position as original position
                originalPosition = selectedItem.transform.position;
                
                // Calculate the offset between the object and the point where the ray hit it
                dragOffset = selectedItem.transform.position - worldPosition;
            }
        }
        
        // DRAG AND DROP THE ITEM --
        // PRESS: PURCHASE; DRAG: MOVE; RELEASE: PLACE
        private void ItemDragAndDrop()
        {
            // CLICK
            //ItemClick();
            
            // DRAGGING
            if (Input.GetMouseButton(0)
                && isDragging
                && selectedItem != null)
            {
                ItemDragging();
            }
        }
        
        // DRAGGING THE ITEM
        private void ItemDragging()
        {
            // Cast a ray, from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Plane for object movement along X-Z
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            float distance;

            // Check for ray--plane intersection
            if (plane.Raycast(ray, out distance))
            {
                // Calculate the intersection point
                Vector3 point = ray.GetPoint(distance);
            
                // Move the object to the intersection point
                selectedItem.transform.position = new Vector3(
                    point.x + dragOffset.x, 
                    selectedItem.transform.position.y, 
                    point.z + dragOffset.z);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (GameManager.instance.shopManager.isDrawerExpanded)
            {
                Debug.Log("Pointer enter");
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (GameManager.instance.shopManager.isDrawerExpanded)
            {
                Debug.Log("Pointer exit");
            }
        }
        
        // CLICK: MAY NOT BE NEEDED
        private void ItemClick()
        {
            // PRESSED DOWN, TODO: MAYBE NO USE AT ALL
            if (Input.GetMouseButtonDown(0))
            {
                // Cast a ray, from the camera to the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                // Check if the ray hits any object (casting triggers allowed)
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("ShopItem"), QueryTriggerInteraction.Collide))
                {
                    // Check if the object is interactable
                    if (hit.collider.CompareTag("ShopItem"))
                    {
                        // Store the selected object (parent)
                        selectedItem = hit.collider.gameObject.transform.parent.gameObject;
                        isDragging = true;
                        
                        // For debugging only
                        Debug.Log(selectedItem.name);
                        
                        // Store original position
                        originalPosition = selectedItem.transform.position;
                        
                        // Calculate the offset between the object and the point where the ray hit it
                        dragOffset = selectedItem.transform.position - hit.point;
                    }
                }
            }
        }
    }
}
