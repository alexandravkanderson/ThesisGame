using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCharacter : MonoBehaviour
{
    [SerializeField] private CharacterAttributes ghostAttributes;
    
    // Start is called before the first frame update
    void Start()
    {
        // Set up the scriptable object
        ghostAttributes = Resources.Load<CharacterAttributes>("ScriptableObject/GhostCharacter");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
