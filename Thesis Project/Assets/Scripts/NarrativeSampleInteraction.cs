using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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
    [SerializeField] private List<Sprite> sequence;
    [SerializeField] private int currentImageIndex = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        // Wiring the game objects
        narrativeText = GameObject.Find("NarrativeText").GetComponent<TextMeshProUGUI>();
        storyboardImage = GameObject.Find("StoryboardImage").GetComponent<Image>();
        
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
            // Display
            narrativeText.gameObject.SetActive(false);
            storyboardImage.gameObject.SetActive(true);

            // Loading images
            Object[] loadedImages = Resources.LoadAll("Sprites/Storyboard", typeof(Sprite));
            Object[] sortedImages = loadedImages.OrderBy(sprite => sprite.name).ToArray();
            
            // Adding images to the sequence (list)
            sequence = new List<Sprite>();
            foreach (var image in sortedImages)
            {
                sequence.Add((Sprite)image);
            }
            
            // Show the first image
            if (sequence.Count > 0)
            {
                storyboardImage.sprite = sequence[currentImageIndex];
            }
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
            Storyboard();
        }
    }

    private void Storyboard()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentImageIndex < sequence.Count - 1)
            {
                currentImageIndex++;
                storyboardImage.sprite = sequence[currentImageIndex];
            }
            else
            {
                LoadAutobattle();
            }
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
