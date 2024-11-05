using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype_v_1_Scripts;
using UnityEngine.Serialization;

namespace Prototype_v_1_Scripts
{
    [CreateAssetMenu
        (
            fileName = "New Shop Item",
            menuName = "Shop Item",
            order = 1)
    ]
    
    public class ShopItemScriptableObject : ScriptableObject
    {
        /*public Item itemType;
        public float modifier = 1;*/
        
        public string shopItemName;
        public string shopItemDescription;
        
        public Sprite shopItemSprite;
        
        public string requiredCurrency;
    }
}
