using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemInteraction : PlayerInteraction, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject shopItemPrefab;
    private GameObject shopItem = null;
    [SerializeField] private float zOffset;
    
    // For description
    [SerializeField] private bool isHoveringOverButton;
    [SerializeField] private GameObject descriptionHolder;
    [SerializeField] private TextMeshProUGUI description;
    
    [SerializeField] private bool isCollidingWithGhost = false;

    protected override void Start()
    {
        base.Start();
        
        tagName = "ShopItem";

        // Wiring the description holder and the text 
        descriptionHolder = transform.parent.GetChild(1).gameObject;
        description = descriptionHolder.GetComponentInChildren<TextMeshProUGUI>();
        
        // Pulling the content of description from the scriptable object
        description.text = shopItemPrefab.GetComponentInChildren<ShopItem>().shopItemAttributes.description;
        
        descriptionHolder.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        
        // Monitoring if the item is colliding with the ghost pawn
        if (shopItem != null && selectedObject != null)
        {
            isCollidingWithGhost = selectedObject.GetComponentInChildren<ShopItem>().isCollidedWithGhost;
        }
        
        // Monitoring if the player is hovering over the button
        if (isHoveringOverButton)
        {
            descriptionHolder.SetActive(true);
        }
        else
        {
            descriptionHolder.SetActive(false);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHoveringOverButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHoveringOverButton = false;
    }

    // When the button of the shop item is down, 
    // create an item at the mouse position
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
        
        switch (itemAttributes.itemType)
        {
            case Item.HP:
                // Add HP to the player
                GameManager.instance.characterGhost.playerHP += itemAttributes.modifier;
                
                // Destroy the button game object
                Destroy(gameObject);
                break;
            case Item.AD:
                // Boost the players attack damage point
                GameManager.instance.characterGhost.AD *= itemAttributes.modifier;
                Debug.Log("Player AD: " + GameManager.instance.characterGhost.AD);
                
                // Destroy the button game object
                Destroy(gameObject);
                break;
            case Item.SP:
                // Boost the players attacking speed
                GameManager.instance.characterGhost.SP *= itemAttributes.modifier;
                Debug.Log("Player SP: " + GameManager.instance.characterGhost.SP);
                
                // Destroy the button game object
                Destroy(gameObject);
                break;
        }
    }
}
