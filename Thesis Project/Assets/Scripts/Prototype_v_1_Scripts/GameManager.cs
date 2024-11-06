using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype_v_1_Scripts;
using UnityEngine.SceneManagement;
using Yarn.Unity;

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
        public CameraManager cameraManager;
        public GridManager gridManager;
        public ShopManager shopManager;
        public LevelManagerHeart levelManager; // NOTE: UNFINISHED
        
        // UI
        public GameObject HUD;
        public ShopCurrencyUI shopCurrencyUI;
        
        // CAMERA
        public Vector3 cameraRotation = new Vector3(45f, 0f, 0f); // Camera rotation angle
        
        // YARN SPINNER
        public DialogueRunner dialogueRunner;
        
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
                        /*cameraManager.StartCameraTransitionToAutobattlerPosition(null); // Start camera transition to autobattler WITH NULL DIALOGUE CASE*/
                        cameraManager.StartCameraTransitionToAutobattlerPosition("Environmental_Level_TestDialogue"); // Start camera transition to autobattler
                        PlayerController.instance.controlType = ControlType.AutoBattler; // Switch control type to auto battler
                        
                        Debug.Log("Switching to AutoBattler"); // Debug
                        break;
                }
            }
        }
        
        // AUTOBATTLE
        public AStar aStar;
        [SerializeField] private bool isAutobattleStarted = false;
        public GameObject restartButton;

        // Start is called before the first frame update
        void Start()
        {
            // Wiring scripts
            cameraManager = GetComponent<CameraManager>();
            gridManager = GetComponent<GridManager>();
            shopManager = GetComponent<ShopManager>();
            
            // UI
            HUD = GameObject.Find("HUDCanvas");
            shopCurrencyUI = FindObjectOfType<ShopCurrencyUI>();
            
            // Wiring Yarn Spinner
            dialogueRunner = FindObjectOfType<DialogueRunner>();
            
            // Initializing current progression to the beginning
            currentProgression = GameProgression.MainMenu;
            
            // Initializing the AB
            aStar = GetComponent<AStar>();
            isAutobattleStarted = false;
            
            // TEMP: SET THE RESTART BUTTON
            restartButton = HUD.transform.GetChild(0).GetChild(3).gameObject;
            restartButton.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        
        // AUTOBATTLE START
        public void StartAutobattle()
        {
            isAutobattleStarted = true;
            
            // Start the autobattle
            PlayerController.instance.MoveToPosition(levelManager.target.transform.position);
            
            // Hide the start button
            HUD.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        }
        
        public void EndAutobattle()
        {
            isAutobattleStarted = false;
            SceneManager.LoadScene(0);
        }
    }
}
