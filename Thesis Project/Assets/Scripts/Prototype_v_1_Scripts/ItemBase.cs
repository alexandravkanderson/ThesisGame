using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Prototype_v_1_Scripts;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Prototype_v_1_Scripts
{
    public class ItemBase : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        // VARIABLES
        [SerializeField] private ShopItemScriptableObject shopItemScriptableObject;
        [SerializeField] private GameObject shopItemPrefab;
        
        [SerializeField] private Image itemImagePrefab;
        [SerializeField] private Image itemImage;
        
        [SerializeField] private float itemImageFollowingSpeed = 10f;
        [SerializeField] private bool isItemImageFollowing;

        public float zOffset;

        // DRAG AND DROP
        private GameObject selectedItem;
        private Vector3 originalPosition;
        
        [SerializeField] private bool isDragging;
        private Vector3 dragOffset;
        
        // Start is called before the first frame update
        void Start()
        {
            itemImagePrefab = Resources.Load<Image>("Prototype_v_1_Resources/Prefabs/ItemImage");
        }

        // Update is called once per frame
        void Update()
        {
            // If the control type is autobattler, activate the shop interaction
            if (PlayerController.instance.controlType == ControlType.AutoBattler)
            {
                ItemDragAndDrop();

                if (Input.GetMouseButtonUp(0))
                {
                    isDragging = false;
                    isItemImageFollowing = false;
                    
                    // Destroy the item image
                }
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
                
                // Instantiate the item in the world space
                selectedItem = Instantiate(shopItemPrefab, worldPosition, Quaternion.Euler(GameManager.instance.cameraRotation));
                
                // Create the item image on the UI canvas
                CreateItemImage();
                
                // Enable dragging for the instantiated item
                isDragging = true;
                
                // Store the initial position as original position
                originalPosition = selectedItem.transform.position;
                
                // Calculate the offset between the object and the point where the ray hit it
                dragOffset = selectedItem.transform.position - worldPosition;
            }
        }

        // CREATE THE ITEM IMAGE ON THE UI CANVAS (FOR DISPLAY ONLY)
        private void CreateItemImage()
        {
            itemImage = Instantiate(itemImagePrefab, GameManager.instance.HUD.transform);
            itemImage.sprite = shopItemScriptableObject.itemSprite;
            
            itemImage.transform.position = Input.mousePosition; // Set the position of the item image to the mouse position
            
            isItemImageFollowing = true; // Enable the item image following
        }
        
        // BUTTON STAY: DRAG AND DROP THE ITEM (3D GAME OBJECT IN THE WORLD SPACE) --
        // PRESS: PURCHASE; DRAG: MOVE; RELEASE: PLACE
        private void ItemDragAndDrop()
        {
            // CLICK
            //ItemClick();
            
            // DRAGGING THE 3D GAME OBJECT
            if (Input.GetMouseButton(0)
                && isDragging
                && selectedItem != null)
            {
                ItemDragging();
            }
            
            // DRAGGING THE ITEM IMAGE (IMAGE FOLLOWING THE MOUSE IN THE UI CANVAS)
            if (isItemImageFollowing)
            {
                ItemImageFollowing();
            }
        }
        
        // DRAGGING THE ITEM 3D GAME OBJECT IN THE WORLD SPACE
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
        
        // DRAGGING THE ITEM IMAGE (IMAGE FOLLOWING THE MOUSE IN THE UI CANVAS)
        private void ItemImageFollowing()
        {
            Vector3 mousePosition = Input.mousePosition;
            
            itemImage.transform.position = Vector3.Lerp(
                itemImage.transform.position, 
                mousePosition, 
                itemImageFollowingSpeed * Time.deltaTime);
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
