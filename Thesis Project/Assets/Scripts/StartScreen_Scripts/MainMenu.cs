using Prototype_v_1_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StartScreen_Scripts
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayGame()
        {
            GameManager.instance.CurrentProgression = GameProgression.Lv0Intro;
            /*SceneManager.LoadScene("Level_0");*/
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
