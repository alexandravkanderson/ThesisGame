using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemInteraction : PlayerInteraction, IPointerDownHandler
{
    public GameObject shopItemPrefab;
    private GameObject shopItem = null;
    [SerializeField] private float zOffset;
    
    [SerializeField] private bool isCollidingWithGhost = false;

    protected override void Start()
    {
        base.Start();
        
        tagName = "ShopItem";
    }

    protected override void Update()
    {
        base.Update();
        
        // Monitoring if the item is colliding with the ghost pawn
        if (shopItem != null && selectedObject != null)
        {
            isCollidingWithGhost = selectedObject.GetComponentInChildren<ShopItem>().isCollidedWithGhost;
        }
    }

    // When the button of the shop item is down, 
    // create an item at the mouse postion
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Button down");
        
        if (!GameManager.instance.isBattleStarted)
        {
            // Get the mouse position in game world
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = zOffset;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            
            // Instantiate the item
            shopItem = Instantiate(shopItemPrefab, worldPosition, Quaternion.identity);
        }
    }

    protected override void ReleasingObject()
    {
        base.ReleasingObject();

        // When releasing the object,
        if (selectedObject != null)
        {
            // If it is colliding with the ghost, boost the ghost, and then destroy item game object
            // If it is NOT colliding, do nothing and destroy item game object
            if (isCollidingWithGhost)
            {
                UsingItem(selectedObject);
                
                Debug.Log("Boosting the Ghost...");
            }
            
            // Destroy the item
            Destroy(selectedObject);
            selectedObject = null;
        }
    }

    // Figuring out what type of the item is using 
    private void UsingItem(GameObject itemObj)
    {
        Debug.Log(itemObj.name);
        ShopItemAttributes itemAttributes = itemObj.GetComponentInChildren<ShopItem>().shopItemAttributes;
        
        switch (itemAttributes.ItemType)
        {
            case Item.HP:
                // Add HP to the player
                GameManager.instance.characterGhost.playerHP += itemAttributes.modifier;
                break;
            case Item.AD:
                break;
            case Item.SP:
                break;
        }
    }
}
