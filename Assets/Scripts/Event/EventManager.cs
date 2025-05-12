using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image eventImage;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TextMeshProUGUI[] choiceTexts;
    [SerializeField] private Button continueButton;
    
    [Header("Event Settings")]
    [SerializeField] private EventData[] availableEvents;
    [SerializeField] private bool useWeightedSelection = true;
    
    private EventData currentEvent;
    private PlayerData playerData;
    
    private void Start()
    {
        playerData = GameManager.Instance.PlayerData;
        
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
            continueButton.onClick.AddListener(ReturnToMap);
        }
        
        ShowRandomEvent();
    }
    
    private void ShowRandomEvent()
    {
        if (availableEvents.Length == 0)
        {
            ReturnToMap();
            return;
        }
        
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
            validEvents.AddRange(availableEvents);
        
        EventData selectedEvent;
        
        if (useWeightedSelection && validEvents.Count > 1)
        {
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
            int randomIndex = Random.Range(0, validEvents.Count);
            selectedEvent = validEvents[randomIndex];
        }
        
        ShowEvent(selectedEvent);
    }
    
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
            eventImage.gameObject.SetActive(false);
        
        // Set up choice buttons
        SetupChoiceButtons(eventData);
        
        // If event should appear only once, mark it as viewed
        if (eventData.oneTimeOnly)
        {
            PlayerPrefs.SetInt("Event_" + eventData.eventId, 1);
            PlayerPrefs.Save();
        }
    }
    
    private void SetupChoiceButtons(EventData eventData)
    {
        foreach (Button button in choiceButtons)
            button.gameObject.SetActive(false);
        
        // Set up buttons for each choice
        for (int i = 0; i < eventData.choices.Length && i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceTexts[i].text = eventData.choices[i].choiceText;
            
            int choiceIndex = i; // Save index for use in lambda expression
            
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
        }
        
        descriptionText.text = resultText;
        
        // Apply rewards and effects
        GameManager.Instance.ApplyEventOutcome(goldChange, soulsChange, healthChange);
        
        if (continueButton != null)
            continueButton.gameObject.SetActive(true);
    }
    
    private void ReturnToMap()
    {
        GameManager.Instance.ChangeState(GameState.Map);
    }
} 