using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Prototype_v_1_Scripts;

namespace Prototype_v_1_Scripts
{
    public class ShopManager : MonoBehaviour
    {
        // DRAWER DISPLAY
        [SerializeField] private GameObject drawerView;

        // DRAWER TOGGLE
        [SerializeField] private Vector3 closedDrawerPosition = new Vector3(0, 80, 0);
        [SerializeField] private Vector3 expandedDrawerPosition = new Vector3(0, 280, 0);

        [SerializeField] private float duration = 2.0f;

        public bool isDrawerExpanded = false;
        public bool isDrawerMoving = false;
        
        // INVENTORY
        public Inventory inventory;

        private void Awake()
        {
            // INVENTORY INITIALIZATION
            inventory = new Inventory();
        }

        // Start is called before the first frame update
        void Start()
        {
            // DISPLAY OF THE DRAWER
            drawerView = GameObject.Find("DrawerView");

            // Set the initial position of the drawer
            drawerView.GetComponent<RectTransform>().anchoredPosition = closedDrawerPosition;
            isDrawerExpanded = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        // TOGGLE DRAWER
        public void ToggleDrawer()
        {
            if (isDrawerExpanded && !isDrawerMoving)
            {
                StartCoroutine(ExecuteToggleDrawer(expandedDrawerPosition, closedDrawerPosition, duration));
            }
            else if (!isDrawerExpanded && !isDrawerMoving)
            {
                StartCoroutine(ExecuteToggleDrawer(closedDrawerPosition, expandedDrawerPosition, duration));
            }
        }

        private IEnumerator ExecuteToggleDrawer(Vector3 startPosition, Vector3 targetPosition, float duration)
        {
            isDrawerMoving = true;

            float elapsedTime = 0.0f;

            while (elapsedTime < duration)
            {
                drawerView.GetComponent<RectTransform>().anchoredPosition =
                    Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Set the final position
            drawerView.GetComponent<RectTransform>().anchoredPosition = targetPosition;
            isDrawerMoving = false;
            isDrawerExpanded = !isDrawerExpanded;
        }

        /*// PURCHASE ITEM todo: USELESS FOR NOW
        public void PurchaseItem(ShopItem item)
        {
            if (inventory.HasCurrency(item.requiredCurrency))
            {
                // Remove the currency from the inventory
                inventory.RemoveCurrency(item.requiredCurrency);
                
                Debug.Log($"Purchased: {item.shopItemName}");
            }
            // If the player does not have the required currency
            else
            {
                Debug.LogError("Insufficient currency");
            }
        }*/
        
    }
    
    // INVENTORY -- CURRENCY -- PURCHASED ITEMS
    public class Inventory
    {
        // CURRENCIES
        private HashSet<string> currencyType = new HashSet<string>();
        
        // SHOP ITEMS
        private List<string> purchasedItems = new List<string>();
        
        public void AddCurrency(string currency)
        {
            currencyType.Add(currency);
            Debug.Log("Currency added: " + currency);
        }
        
        public bool HasCurrency(string currency)
        {
            return currencyType.Contains(currency);
        }
        
        // PURCHASED ITEMS -- REMOVE CURRENCY
        public void RemoveCurrency(string currency)
        {
            // If the currency is in the inventory, remove it
            if (currencyType.Contains(currency))
            {
                currencyType.Remove(currency);
                Debug.Log("Currency removed: " + currency);
            }
            // If the currency is NOT in the inventory, log an error
            else
            {
                Debug.LogError("Currency not found: " + currency);
            }
        }
        
        // PURCHASED ITEMS -- ADD ITEM
        public void AddPurchasedItem(string item)
        {
            purchasedItems.Add(item);
            Debug.Log("Item added to inventory: " + item);
        }
        
        public bool HasPurchasedItem(string item)
        {
            return purchasedItems.Contains(item);
        }
    }
    
    /*// SHOP ITEM -- todo: USELESS FOR NOW (OR EVEN FOREVER)
    public class ShopItem
    {
        public string shopItemName;
        public string requiredCurrency;
        
        public GameObject shopItemPrefab;

        public ShopItem(string shopItemName, string requiredCurrency, GameObject shopItemPrefab)
        {
            this.shopItemName = shopItemName;
            this.requiredCurrency = requiredCurrency;
            this.shopItemPrefab = shopItemPrefab;
        }
    }*/
}
