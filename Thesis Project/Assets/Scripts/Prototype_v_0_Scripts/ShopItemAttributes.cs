using JetBrains.Annotations;
using UnityEngine;

namespace Prototype_v_0_Scripts
{
    public enum Item
    {
        HP,
        AD,
        SP
    }

    [CreateAssetMenu
        (
            fileName = "New Item Attribute",
            menuName = "Shop Item Attribute",
            order = 0)
    ]
    public class ShopItemAttributes : ScriptableObject
    {
        public Item itemType;
        public float modifier = 1;
        [CanBeNull] public string name;
        [CanBeNull] public string description;
    }
}