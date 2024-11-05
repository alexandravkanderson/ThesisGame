using System.Collections;
using System.Collections.Generic;
using Prototype_v_1_Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopCurrencyUI : MonoBehaviour
{
    // SHOP CURRENCY PREFAB
    public GameObject currencyPrefab;
    
    // CURRENCY HOLDER
    [SerializeField] private Transform currencyHolder;
    
    // CURRENCIES
    [SerializeField] private Dictionary<string, GameObject> collectedCurrencies;
    
    // Start is called before the first frame update
    void Start()
    {
        // SET UP
        currencyHolder = transform;
        collectedCurrencies = new Dictionary<string, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInventoryDisplay(string currency, Sprite currencySprite)
    {
        // If the currency is not in the dictionary, instantiate it
        if (!collectedCurrencies.ContainsKey(currency))
        {
            GameObject newCurrency = Instantiate(currencyPrefab, currencyHolder);
            newCurrency.name = currency;
            newCurrency.GetComponent<Image>().sprite = currencySprite;
            
            // Add the new currency to the dictionary
            collectedCurrencies[currency] = newCurrency;
            
            Debug.Log("Updated inventory display for: " + currency);
        }
    }
    
    public void RemoveInventoryDisplay(string currency)
    {
        // If the currency is in the dictionary, remove it
        if (collectedCurrencies.ContainsKey(currency))
        {
            Destroy(collectedCurrencies[currency]); // Destroy the currency
            collectedCurrencies.Remove(currency);   // Remove the currency from the dictionary
            
            Debug.Log("Removed inventory display for: " + currency);
        }
    }
}
