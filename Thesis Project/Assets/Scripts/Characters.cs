using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{
    [SerializeField] private string scriptableObjectName;
    [SerializeField] private CharacterAttributes characterAttributes;
    
    // For attributes
    [SerializeField] private int HP; // Health point
    [SerializeField] private int AD; // Attach damage
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Set up the scriptable object
        characterAttributes = Resources.Load<CharacterAttributes>("ScriptableObject/" + scriptableObjectName);

        HP = characterAttributes.healthPoint;
        AD = characterAttributes.attackDamage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
