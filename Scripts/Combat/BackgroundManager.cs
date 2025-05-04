using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    [Header("Background Settings")]
    [SerializeField] private Image backgroundImage; // Reference to UI Image component
    [SerializeField] private Sprite[] backgroundSprites; // Array of possible background sprites
    
    private void Start()
    {
        SetRandomBackground();
    }
    
    public void SetRandomBackground()
    {
        if (backgroundImage == null || backgroundSprites == null || backgroundSprites.Length == 0)
            return;
        
        int randomIndex = Random.Range(0, backgroundSprites.Length);
        
        backgroundImage.sprite = backgroundSprites[randomIndex];
    }
    
    public void SetBackground(int index)
    {
        if (backgroundImage == null || backgroundSprites == null || backgroundSprites.Length == 0)
            return;
        
        if (index < 0 || index >= backgroundSprites.Length)
            return;
        
        backgroundImage.sprite = backgroundSprites[index];
    }
} 