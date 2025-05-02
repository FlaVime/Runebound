using UnityEngine;
using UnityEngine.EventSystems;

public class NodeView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    public NodeModel model { get; private set; }
    
    private Color visitedColor = Color.gray;
    private Color lockedColor = Color.white;
    private Color unlockedColor = Color.green;
    
    private MapSystem mapSystem;
    
    public void Initialize(NodeModel model, MapSystem mapSystem)
    {
        this.model = model;
        this.mapSystem = mapSystem;
        
        UpdateVisuals();
    }
    
    private void OnMouseDown()
    {
        if (!model.isUnlocked) return;
        
        mapSystem.SelectNode(model.id);
    }
    
    public void UpdateVisuals()
    {
        if (model.isVisited)
        {
            spriteRenderer.color = visitedColor;
        }
        else if (model.isUnlocked)
        {
            spriteRenderer.color = unlockedColor;
        }
        else
        {
            spriteRenderer.color = lockedColor;
        }
    }
    
    public void SetNodeType(NodeData data)
    {
        if (data != null && data.nodeIcon != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = data.nodeIcon;
        }
    }
}