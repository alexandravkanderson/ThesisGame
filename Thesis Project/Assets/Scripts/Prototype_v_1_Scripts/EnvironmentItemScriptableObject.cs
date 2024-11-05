using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype_v_1_Scripts;

namespace Prototype_v_1_Scripts
{
    [CreateAssetMenu
        (
            fileName = "New Environment Item",
            menuName = "Environment Item",
            order = 0)
    ]
    
    public class EnvironmentItemScriptableObject : ScriptableObject
    {
        public string environmentItemName;
        public string environmentItemDescription;
        
        public Sprite environmentItemSprite;

    }
}
