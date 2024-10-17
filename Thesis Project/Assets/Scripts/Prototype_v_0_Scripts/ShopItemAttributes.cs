using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

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
