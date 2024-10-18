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
                        SceneManager.LoadSceneAsync(1);
                        currentProgression = GameProgression.Lv0Intro;
                        break;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Initializing current progression to the beginning
            currentProgression = GameProgression.MainMenu;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
