using UnityEngine;

// ScriptableObject for storing event data
// Allows creating and configuring events through Unity editor
[CreateAssetMenu(fileName = "NewEvent", menuName = "Runebound/Event")]
public class EventData : ScriptableObject
{
    [Header("Event Information")]
    public string eventId;          // Unique identifier for the event
    public string title;            // Event title
    [TextArea(3, 10)]
    public string description;      // Main event description
    public Sprite eventImage;       // Event image
    
    [System.Serializable]
    public class Choice
    {
        [Header("Choice Text")]
        public string choiceText;       // Text on the choice button
        [TextArea(3, 8)]
        public string resultText;       // Result text after making this choice
        
        [Header("Rewards and Effects")]
        public int goldChange;          // How much gold the player gets (can be negative)
        public int soulsChange;         // How much souls the player gets (can be negative)
        public int healthChange;        // Health change (can be negative for damage)
        
        [Header("Random Outcome (Optional)")]
        public bool hasRandomOutcome;   // Whether this choice has a random outcome
        [Range(0, 100)]
        public int successChance = 50;  // Chance of success (0-100%)
        [TextArea(3, 8)]
        public string successText;      // Text for successful outcome
        [TextArea(3, 8)]
        public string failureText;      // Text for failed outcome
        
        // Additional rewards for success/failure outcomes
        public int successGoldChange;
        public int successSoulsChange;
        public int successHealthChange;
        
        public int failureGoldChange;
        public int failureSoulsChange;
        public int failureHealthChange;
    }
    
    [Header("Available Choices")]
    public Choice[] choices;    // Array of choices for the player
    
    [Header("Event Settings")]
    [Range(1, 10)]
    public int weight = 1;             // Weight for random selection (higher = more frequent)
    public bool oneTimeOnly = false;   // Event appears only once
} 