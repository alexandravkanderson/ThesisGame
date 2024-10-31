using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype_v_1_Scripts;

namespace Prototype_v_1_Scripts
{
    public enum Item
    {
        HP,
        AD,
        SP
    }
    
    [CreateAssetMenu
        (
            fileName = "New Item",
            menuName = "Shop Item",
            order = 0)
    ]
    
    public class ShopItem : ScriptableObject
    {
        /*public Item itemType;
        public float modifier = 1;*/
        
        public string itemName;
        public string description;
    }
}
