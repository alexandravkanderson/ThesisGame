using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype_v_1_Scripts
{
    public enum ControlType
    {
        EnvironmentalLevel,
        AutoBattler
    }

    public class PlayerController : MonoBehaviour
    {
        // SINGLETON
        public static PlayerController instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        
        // PLAYER CONTROL TYPE
        public ControlType controlType;

        // Start is called before the first frame update
        void Start()
        {
            // Initializing the controller as environmental level
            controlType = ControlType.EnvironmentalLevel;
        }

        // Update is called once per frame
        void Update()
        {
            // ENVIRONMENTAL LEVEL
            if (controlType == ControlType.EnvironmentalLevel)
            {
                
            }
            // AUTOBATTLER
            else if (controlType == ControlType.AutoBattler)
            {
                
            }
        }
    }
}
