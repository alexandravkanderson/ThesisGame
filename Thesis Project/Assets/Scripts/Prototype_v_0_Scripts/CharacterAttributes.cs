using UnityEngine;

namespace Prototype_v_0_Scripts
{
    [CreateAssetMenu
        (        
            fileName = "New Character Attribute",        
            menuName = "Game Character Attribute",        
            order = 0)
    ]

    public class CharacterAttributes : ScriptableObject
    {
        public float healthPoint;
        public float attackDamage;
        public float attackSpeed;
    }
}
