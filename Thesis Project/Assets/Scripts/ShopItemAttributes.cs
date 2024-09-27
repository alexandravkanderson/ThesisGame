using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Item ItemType;
    public float modifier = 1;
}
