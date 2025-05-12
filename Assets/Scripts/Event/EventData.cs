using UnityEngine;

[CreateAssetMenu(fileName = "NewEvent", menuName = "Runebound/Event")]
public class EventData : ScriptableObject
{
    [Header("Event Information")]
    public string eventId;
    public string title;
    [TextArea(3, 10)]
    public string description;
    public Sprite eventImage;
    
    [System.Serializable]
    public class Choice
    {
        [Header("Choice Text")]
        public string choiceText;
        [TextArea(3, 8)]
        public string resultText;
        
        [Header("Rewards and Effects")]
        public int goldChange;
        public int soulsChange;
        public int healthChange;
        
        [Header("Random Outcome (Optional)")]
        public bool hasRandomOutcome; // Whether this choice has a random outcome
        [Range(0, 100)]
        public int successChance = 50; // Chance of success (0-100%)
        [TextArea(3, 8)]
        public string successText; // Text for successful outcome
        [TextArea(3, 8)]
        public string failureText; // Text for failed outcome
        
        public int successGoldChange;
        public int successSoulsChange;
        public int successHealthChange;
        
        public int failureGoldChange;
        public int failureSoulsChange;
        public int failureHealthChange;
    }
    
    [Header("Available Choices")]
    public Choice[] choices; // Array of choices for the player
    
    [Header("Event Settings")]
    [Range(1, 10)]
    public int weight = 1; // Weight for random selection (higher = more frequent)
    public bool oneTimeOnly = false; 
} 