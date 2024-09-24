using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu
    (        
        fileName = "New Character Attribute",        
        menuName = "Game Character Attribute",        
        order = 0)
]

public class CharacterAttributes : ScriptableObject
{
    public int healthPoint;
    public int attackDamage;
    
}
