using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu
    (        
        fileName = "New Character",        
        menuName = "Game Character",        
        order = 0)
]

public class CharacterAttributes : ScriptableObject
{
    public int healthPoint;
    public int attackPoint;
    
}
