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
        [SerializeField] private ShopItem shopItemScriptableObject;
        [SerializeField] private GameObject shopItemPrefab;

        public float zOffset;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

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
                shopItemPrefab = Instantiate(shopItemPrefab, worldPosition, Quaternion.Euler(new Vector3(45, 0, 0)));
            }
        }
        
        // 

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
    }
}
