using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype_v_1_Scripts;
using UnityEngine.SceneManagement;

namespace Prototype_v_1_Scripts
{
    public enum GameProgression
    {
        MainMenu,
        Lv0Intro,
        Lv1HeartEnvironment,
        Lv1HeartAutobattler,
        TBD
    }
    
    public class GameManager : MonoBehaviour
    {
        // SINGLETON
        public static GameManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // WIRING SCRIPTS
        [SerializeField] private CameraManager cameraManager;
        
        // MONITORING GAME PROGRESSION
        [SerializeField] private GameProgression currentProgression;

        public GameProgression CurrentProgression
        {
            get
            {
                return currentProgression;
            }
            set
            {
                // Update the current progression to the new value
                currentProgression = value;
                
                // Load scenes
                switch (currentProgression)
                {
                    case GameProgression.Lv0Intro:
                        SceneManager.LoadSceneAsync(1); // Load the intro level scene
                        currentProgression = GameProgression.Lv0Intro;
                        break;
                    
                    case GameProgression.Lv1HeartEnvironment:
                        PlayerController.instance.controlType = ControlType.EnvironmentalLevel; // Switch control type to environmental level
                        break;
                    
                    case GameProgression.Lv1HeartAutobattler:
                        cameraManager.StartCameraTransitionToAutobattler(); // Start camera transition to autobattler
                        PlayerController.instance.controlType = ControlType.AutoBattler; // Switch control type to auto battler
                        
                        Debug.Log("Switching to AutoBattler"); // Debug
                        break;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Wiring scripts
            cameraManager = GetComponent<CameraManager>();
            
            // Initializing current progression to the beginning
            currentProgression = GameProgression.MainMenu;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
