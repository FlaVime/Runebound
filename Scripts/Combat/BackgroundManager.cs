using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    [Header("Background Settings")]
    [SerializeField] private Image backgroundImage; // Reference to UI Image component
    [SerializeField] private Sprite[] backgroundSprites; // Array of possible background sprites
    
    private void Awake()
    {
        // Check if components are assigned
        if (backgroundImage == null)
        {
            Debug.LogError("BackgroundManager: backgroundImage is not assigned!");
        }
        
        if (backgroundSprites == null || backgroundSprites.Length == 0)
        {
            Debug.LogError("BackgroundManager: backgroundSprites are not assigned or array is empty!");
        }
    }
    
    private void Start()
    {
        // Set random background at combat start
        SetRandomBackground();
    }
    
    /// <summary>
    /// Sets a random background from available options
    /// </summary>
    public void SetRandomBackground()
    {
        if (backgroundImage == null || backgroundSprites == null || backgroundSprites.Length == 0)
            return;
        
        // Choose random sprite from array
        int randomIndex = Random.Range(0, backgroundSprites.Length);
        
        // Set selected sprite as background
        backgroundImage.sprite = backgroundSprites[randomIndex];
    }
    
    /// <summary>
    /// Sets specific background by index
    /// </summary>
    public void SetBackground(int index)
    {
        if (backgroundImage == null || backgroundSprites == null || backgroundSprites.Length == 0)
            return;
        
        // Check if index is valid
        if (index < 0 || index >= backgroundSprites.Length)
            return;
        
        // Set selected sprite as background
        backgroundImage.sprite = backgroundSprites[index];
    }
} 