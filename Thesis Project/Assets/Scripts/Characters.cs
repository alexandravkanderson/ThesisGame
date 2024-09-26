using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{
    [SerializeField] private string scriptableObjectName;
    [SerializeField] private CharacterAttributes characterAttributes;
    
    // For attributes
    [SerializeField] protected float HP;      // Health point
    [SerializeField] protected float AD;      // Attach damage
    [SerializeField] protected float SP;    // Attach speed
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Set up the scriptable object
        characterAttributes = Resources.Load<CharacterAttributes>("ScriptableObject/" + scriptableObjectName);

        HP = characterAttributes.healthPoint;
        AD = characterAttributes.attackDamage;
        SP = characterAttributes.attackSpeed;
    }
}
