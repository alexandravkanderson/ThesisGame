using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    // For colliding
    public bool isCollidedWithGhost;
    
    // For attributes
    public ShopItemAttributes shopItemAttributes;

    private void Start()
    {
        //Debug.Log("Item generated!");
    }

    // Monitoring if the item is colliding with the ghost pawn
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.CompareTag("Player"))
        {
            
            isCollidedWithGhost = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.gameObject.CompareTag("Player"))
        {
            isCollidedWithGhost = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isCollidedWithGhost = false;
    }
}
