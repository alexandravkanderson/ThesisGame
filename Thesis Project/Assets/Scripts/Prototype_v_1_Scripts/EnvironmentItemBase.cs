using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype_v_1_Scripts;

namespace Prototype_v_1_Scripts
{
    public class EnvironmentItemBase : MonoBehaviour
    {
        // ITEM
        private SpriteRenderer spriteRenderer;
        
        // SCRIPTABLE OBJECT
        public EnvironmentItemScriptableObject environmentItemScriptableObject;

        // Start is called before the first frame update
        void Start()
        {
            // ITEM
            spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            
            // Set the sprite of the environment item
            spriteRenderer.sprite = environmentItemScriptableObject.environmentItemSprite;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
