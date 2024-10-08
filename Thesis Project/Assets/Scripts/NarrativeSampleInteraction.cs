using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum NarrativeType
{
    TextBased,
    Storyboard
}

public class NarrativeSampleInteraction : MonoBehaviour
{
    // TYPE
    public NarrativeType narrativeType;
    
    // Text
    [SerializeField] private TextMeshProUGUI narrativeText;
    
    // 0: Fist dialogue displayed
    // 1: Second dialogue displayed
    // 2: Third dialogue displayed -> the next click should be loading battle scene
    private int isNarrativeFinished = 0;

    // Storyboard
    [SerializeField] private Image storyboardImage;
    
    // Start is called before the first frame update
    void Start()
    {
        // Wiring the game objects
        narrativeText = GameObject.Find("NarrativeText").GetComponent<TextMeshProUGUI>();
        
        // Text
        if (narrativeType == NarrativeType.TextBased)
        {
            // Display
            narrativeText.gameObject.SetActive(true);
            storyboardImage.gameObject.SetActive(false);
            
            // The first sentence 
            narrativeText.fontStyle = FontStyles.Italic;
            narrativeText.text = "What’s the last thing I remember?";
        }
        // Storyboard
        else if (narrativeType == NarrativeType.Storyboard)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (narrativeType == NarrativeType.TextBased)
        {
            TextBased();
        }
        else if (narrativeType == NarrativeType.Storyboard)
        {
            
        }
    }

    private void TextBased()
    {
        // The first click, show second dialogue
        if (Input.GetMouseButtonDown(0) && isNarrativeFinished == 0)
        {
            narrativeText.fontStyle = FontStyles.Normal;
            
            narrativeText.text = "All I remember is pain and darkness, flashing before my eyes like lightning, then emerging from nothing. \n" +
                                 "Your eyes… were the first thing I saw after my whole world went black. \n" +
                                 "Why can't I remember anything before that moment?";
            isNarrativeFinished++;
        }
        // The second click, show third dialogue
        else if (Input.GetMouseButtonDown(0) && isNarrativeFinished == 1)
        {
            narrativeText.text = "I feel uneasy, terrified even, at the notion that I can barely remember who I am.";
            isNarrativeFinished++;
        }
        else if (Input.GetMouseButtonDown(0) && isNarrativeFinished == 2)
        {
            LoadAutobattle();
        }
    }
    
    // Load the auto-battle prototype scene
    private void LoadAutobattle()
    {
        SceneManager.LoadScene("Level_1");
    }
}
