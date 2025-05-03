using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// EventManager - main class that handles event display and player choices
public class EventManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;       // Event title text
    [SerializeField] private TextMeshProUGUI descriptionText; // Event description text
    [SerializeField] private Image eventImage;                // Event image
    [SerializeField] private Button[] choiceButtons;          // Choice buttons
    [SerializeField] private TextMeshProUGUI[] choiceTexts;   // Choice button texts
    [SerializeField] private Button continueButton;           // Continue button (to return to map)
    
    [Header("Event Settings")]
    [SerializeField] private EventData[] availableEvents;     // Array of all available events
    [SerializeField] private bool useWeightedSelection = true;// Use weighted random selection
    
    private EventData currentEvent;                           // Currently displayed event
    private PlayerData playerData;                            // Reference to player data
    
    // Initialize on start
    private void Start()
    {
        // Get reference to player data
        playerData = GameManager.Instance.PlayerData;
        
        // Hide continue button initially
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
            continueButton.onClick.AddListener(ReturnToMap);
        }
        
        // Show a random event
        ShowRandomEvent();
    }
    
    // Show a random event from available events
    private void ShowRandomEvent()
    {
        if (availableEvents.Length == 0)
        {
            Debug.LogError("No available events!");
            ReturnToMap();
            return;
        }
        
        // Create a list of available events, filtering one-time events
        List<EventData> validEvents = new List<EventData>();
        foreach (EventData eventData in availableEvents)
        {
            // Skip one-time events that have already been shown
            if (eventData.oneTimeOnly && PlayerPrefs.HasKey("Event_" + eventData.eventId))
            {
                continue;
            }
            
            validEvents.Add(eventData);
        }
        
        // If no valid events remain, use all events as fallback
        if (validEvents.Count == 0)
        {
            Debug.LogWarning("No valid events found. Using all events as fallback.");
            validEvents.AddRange(availableEvents);
        }
        
        // Select event randomly
        EventData selectedEvent;
        
        if (useWeightedSelection && validEvents.Count > 1)
        {
            // Weighted random selection (events with higher weight appear more frequently)
            int totalWeight = 0;
            foreach (var eventData in validEvents)
            {
                totalWeight += eventData.weight;
            }
            
            int randomValue = Random.Range(0, totalWeight);
            int currentWeight = 0;
            
            selectedEvent = validEvents[0]; // Default
            
            foreach (var eventData in validEvents)
            {
                currentWeight += eventData.weight;
                if (randomValue < currentWeight)
                {
                    selectedEvent = eventData;
                    break;
                }
            }
        }
        else
        {
            // Simple random selection
            int randomIndex = Random.Range(0, validEvents.Count);
            selectedEvent = validEvents[randomIndex];
        }
        
        // Show the selected event
        ShowEvent(selectedEvent);
    }
    
    // Display specific event
    private void ShowEvent(EventData eventData)
    {
        currentEvent = eventData;
        
        // Set up UI elements
        titleText.text = eventData.title;
        descriptionText.text = eventData.description;
        
        // Set up image
        if (eventData.eventImage != null)
        {
            eventImage.sprite = eventData.eventImage;
            eventImage.gameObject.SetActive(true);
        }
        else
        {
            eventImage.gameObject.SetActive(false);
        }
        
        // Set up choice buttons
        SetupChoiceButtons(eventData);
        
        // If event should appear only once, mark it as viewed
        if (eventData.oneTimeOnly)
        {
            PlayerPrefs.SetInt("Event_" + eventData.eventId, 1);
            PlayerPrefs.Save();
        }
    }
    
    // Set up choice buttons
    private void SetupChoiceButtons(EventData eventData)
    {
        // First hide all buttons
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
        
        // Set up buttons for each choice
        for (int i = 0; i < eventData.choices.Length && i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceTexts[i].text = eventData.choices[i].choiceText;
            
            int choiceIndex = i; // Save index for use in lambda expression
            
            // Clear previous listeners and add new one
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => HandleChoice(choiceIndex));
        }
    }
    
    // Handle player choice
    private void HandleChoice(int choiceIndex)
    {
        if (currentEvent == null || choiceIndex >= currentEvent.choices.Length)
        {
            return;
        }
        
        // Get selected choice
        EventData.Choice choice = currentEvent.choices[choiceIndex];
        
        // Hide all choice buttons
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
        
        // Process result with random outcome check if required
        bool success = true;
        string resultText = choice.resultText;
        int goldChange = choice.goldChange;
        int soulsChange = choice.soulsChange;
        int healthChange = choice.healthChange;
        
        if (choice.hasRandomOutcome)
        {
            // Generate random number from 1 to 100
            int roll = Random.Range(1, 101);
            success = roll <= choice.successChance;
            
            // Show appropriate result text
            resultText = success ? choice.successText : choice.failureText;
            
            // Apply outcome-specific rewards
            if (success)
            {
                goldChange = choice.successGoldChange;
                soulsChange = choice.successSoulsChange;
                healthChange = choice.successHealthChange;
            }
            else
            {
                goldChange = choice.failureGoldChange;
                soulsChange = choice.failureSoulsChange;
                healthChange = choice.failureHealthChange;
            }
            
            Debug.Log($"Random outcome check: rolled {roll}, needed {choice.successChance} or less, result: {(success ? "success" : "failure")}");
        }
        
        // Update description text with result
        descriptionText.text = resultText;
        
        // Apply rewards and effects
        // Gold
        if (goldChange != 0)
        {
            playerData.AddGold(goldChange);
            Debug.Log($"Gold change: {(goldChange > 0 ? "+" : "")}{goldChange}, total: {playerData.gold}");
        }
        
        // Souls
        if (soulsChange != 0)
        {
            playerData.AddSouls(soulsChange);
            Debug.Log($"Souls change: {(soulsChange > 0 ? "+" : "")}{soulsChange}, total: {playerData.souls}");
        }
        
        // Health
        if (healthChange != 0)
        {
            if (healthChange > 0)
                playerData.Heal(healthChange);
            else
                playerData.TakeDamage(-healthChange);
                
            Debug.Log($"Health change: {(healthChange > 0 ? "+" : "")}{healthChange}, total: {playerData.currentHealth}/{playerData.maxHealth}");
        }
        
        // Save game state
        GameManager.Instance.SaveGame();
        
        // Show continue button
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
        }
    }
    
    // Return to map
    private void ReturnToMap()
    {
        // Use SceneManager to return to the map scene
        SceneManager.LoadScene("Map");
        // Or alternatively use GameManager if you prefer that approach:
        // GameManager.Instance.ChangeState(GameState.Map);
    }
} 