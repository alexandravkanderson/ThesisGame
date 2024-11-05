using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollisionDetector : MonoBehaviour
{
    // COLLISION
    public bool isCollidingWithPlayer { get; private set; } = false;
    
    // TRIGGER
    public bool isTriggeringWithPlayer { get; private set; } = false;

    // COLLISION
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }
    
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        isCollidingWithPlayer = false;
    }
    
    // TRIGGER
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isTriggeringWithPlayer = true;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isTriggeringWithPlayer = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        isTriggeringWithPlayer = false;
    }
}
